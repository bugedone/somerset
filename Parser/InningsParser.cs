using System.Collections.Generic;
using System.Linq;
using Spider.Domain;
using System.Text.RegularExpressions;
using log4net;
using Spider.Parser.HowOut;
using HtmlAgilityPack;

namespace Spider.Parser
{
    class InningsParser
    {
        private static readonly ILog Log = LogManager.GetLogger("Spider");
        private readonly CricketMatch _match;

        public InningsParser(CricketMatch match)
        {
            _match = match;
        }

        public Innings Parse(HtmlNode battingNode, HtmlNode bowlingNode)
        {
            Innings innings = new Innings();
            AddBatting(battingNode, innings);
            AddBowling(bowlingNode, innings);
            return innings;
        }

        private void AddBatting(HtmlNode battingNode, Innings innings)
        {
            HtmlNodeCollection rows = battingNode.SelectNodes(".//tr");
            ExtractWhoseInnings(rows[0], innings);

            innings.Batting = new List<BatsmanInnings>();

            int i = 1;
            while (i < rows.Count)
            {
                HtmlNode cell = rows[i].SelectSingleNode("./td[1]");
                HtmlNode playerNode = cell.SelectSingleNode("./a");
                if (playerNode == null && (cell.InnerText == "Extras" || cell.InnerText == "Total" || cell.InnerText == "Innings Forfeited"))
                    break;
                BatsmanInnings batsman = ParseBatsmanInnings(rows[i], (innings.BattingTeam == "Somerset" && innings.TeamInningsNumber == 1));
                batsman.Number = i;
                batsman.Team = innings.BattingTeam;
                innings.Batting.Add(batsman);

                i++;
            }

            while (i < rows.Count)
            {
                HtmlNode cell = rows[i].SelectSingleNode("./td[1]");
                string label = (cell != null ? cell.InnerText : null);
                if (string.IsNullOrEmpty(label))
                {
                    i++;
                    continue;
                }

                if (label == "Innings Forfeited")
                {
                    innings.Forfeited = true;
                    break;
                }

                if (label == "Extras")
                    ProcessExtras(rows[i], innings);
                if (label == "Total")
                    ProcessTotal(rows[i], innings);
                if (label == "Fall of wickets:")
                {
                    ProcessFallOfWickets(rows[i + 1], innings);
                    i++;
                }

                i++;
            }
        }

        private static void ExtractWhoseInnings(HtmlNode headerRow, Innings innings)
        {
            HtmlNode cell = headerRow.SelectSingleNode("./td[1]");
            string content = cell.InnerText;
            if (string.IsNullOrEmpty(content))
                return;

            content = content.Trim();
            if (content.EndsWith("first innings"))
            {
                innings.BattingTeam = content.Substring(0, content.Length - 14).TrimEnd();
                innings.TeamInningsNumber = 1;
            }
            else if (content.EndsWith("second innings"))
            {
                innings.BattingTeam = content.Substring(0, content.Length - 15).TrimEnd();
                innings.TeamInningsNumber = 2;
            }
            else if (content.EndsWith("second innings (following on)"))
            {
                innings.BattingTeam = content.Substring(0, content.Length - 30).TrimEnd();
                innings.TeamInningsNumber = 2;
            }
            else if (content.EndsWith("innings"))
            {
                innings.BattingTeam = content.Substring(0, content.Length - 8).TrimEnd();
                innings.TeamInningsNumber = 1;
            }
        }

        private static void ProcessExtras(HtmlNode row, Innings innings)
        {
            string extrasText = row.SelectSingleNode("./td[2]").InnerText;

            int total;
            if (int.TryParse(row.SelectSingleNode("./td[3]").InnerText, out total))
                innings.Extras = total;

            var m = Regex.Match(extrasText, @"\(?(?:(?'byes'\d+)\sb)?(?:,\s)?(?:(?'legbyes'\d+)\slb)?(?:,\s)?(?:(?'noballs'\d+)\snb)?(?:,\s)?(?:(?'wides'\d+)\sw)?\)?");
            int byes;
            int legByes;
            int noBalls;
            int wides;
            if (int.TryParse(m.Groups["byes"].Value, out byes))
                innings.Byes = byes;
            if (int.TryParse(m.Groups["legbyes"].Value, out legByes))
                innings.LegByes = legByes;
            if (int.TryParse(m.Groups["noballs"].Value, out noBalls))
                innings.NoBalls = noBalls;
            if (int.TryParse(m.Groups["wides"].Value, out wides))
                innings.Wides = wides;
        }

        private void ProcessTotal(HtmlNode row, Innings innings)
        {
            string wicketsAndOvers = row.SelectSingleNode("./td[2]").InnerText;
            string totalText = row.SelectSingleNode("./td[3]").InnerText;
            int wickets, overs, balls, total;

            if (!int.TryParse(totalText, out total))
                total = 0;

            var m = Regex.Match(wicketsAndOvers, @"\((?:(?'wickets'(?:\d+|no))\swickets?|(?'allout'all\sout))(?:,\s(?'dec'declared))?(?:,\s(?'mins'\d+) minutes)?(?:,\s(?:innings\sclosed,\s)?(?'overs'\d+)\.?(?'balls'\d)?\sovers)?\)");
            if (!string.IsNullOrEmpty(m.Groups["allout"].Value))
                innings.AllOut = true;
            else if (m.Groups["wickets"].Value == "no")
                innings.Wickets = 0;
            else if (int.TryParse(m.Groups["wickets"].Value, out wickets))
                innings.Wickets = wickets;
            if (!int.TryParse(m.Groups["overs"].Value, out overs))
                overs = -1;
            if (!int.TryParse(m.Groups["balls"].Value, out balls))
                balls = 0;
            if (!string.IsNullOrEmpty(m.Groups["dec"].Value))
                innings.Declared = true;

            if (overs != -1)
            {
                innings.Overs = balls == 0 ? overs.ToString() : string.Format("{0}.{1}", overs, balls);
                innings.BallsBowled = (overs * _match.BallsPerOver) + balls;
            }
            innings.Total = total;
        }

        private static void ProcessFallOfWickets(HtmlNode row, Innings innings)
        {
            List<FallOfWicket> fallOfWickets = new List<FallOfWicket>();

            // If we ever decide to add support for who the wicket was...
            MatchCollection matches = Regex.Matches(row.FirstChild.InnerHtml, @"(?'wicket'\d{1,2})-(?'runs'(?:\?|\d{1,3}))(?:\s\(<a\shref=""/Archive/Players/\d+/\d+/(?'player'\d+).html"">[^<]+</a>,\s(?'overs'\d+\.?\d?)\sov\))?");
            //MatchCollection matches = Regex.Matches(content, @"(?'wicket'\d{1,2})-(?'runs'(?:\?|\d{1,3}))");
            foreach (Match fall in matches)
            {
                int wicket = int.Parse(fall.Groups["wicket"].Value);
                int? runs = null;
                if (fall.Groups["runs"].Value != "?")
                    runs = int.Parse(fall.Groups["runs"].Value);

                string overs = fall.Groups["overs"].Value;
                string player = fall.Groups["player"].Value;

                FallOfWicket fow = new FallOfWicket
                {
                    Wicket = wicket,
                    Runs = runs,
                    Overs = overs
                };
                if (!string.IsNullOrEmpty(player))
                    fow.OutBatsmanId = PlayerDetails.GenerateId(int.Parse(player));

                fallOfWickets.Insert(wicket - 1, fow);
            }

            innings.FallOfWickets = fallOfWickets;
        }

        private BatsmanInnings ParseBatsmanInnings(HtmlNode row, bool saveCaptainAndKeeper)
        {
            BatsmanInnings innings = new BatsmanInnings();
            HtmlNodeCollection cells = row.SelectNodes("./td");

            HtmlNode player = cells[0].LastChild;

            if (player.NodeType == HtmlNodeType.Element && player.Name == "a")
            {
                string batsmanId = FindPlayerId(player);
                innings.PlayerId = batsmanId;

                if (saveCaptainAndKeeper)
                {
                    string icons = "";
                    if (cells[0].FirstChild.NodeType == HtmlNodeType.Text)
                    {
                        icons = cells[0].FirstChild.InnerText;
                    }

                    bool isKeeper = icons.Contains("+");
                    bool isCaptain = icons.Contains("*");

                    if (isKeeper)
                        SetKeeper(batsmanId);
                    if (isCaptain)
                        SetCaptain(batsmanId);
                }
            }

            string runs = null;
            if (cells.Count >= 3)
                runs = cells[2].InnerText.Replace("&nbsp;", "").Replace("-", "");
            string balls = null;
            if (cells.Count >= 4)
                balls = cells[3].InnerText.Replace("&nbsp;", "").Replace("-", "");
            string mins = null;
            if (cells.Count >= 4)
                mins = cells[4].InnerText.Replace("&nbsp;", "").Replace("-", "");
            string fours = null;
            if (cells.Count >= 4)
                fours = cells[5].InnerText.Replace("&nbsp;", "").Replace("-", "");
            string sixes = null;
            if (cells.Count >= 4)
                sixes = cells[6].InnerText.Replace("&nbsp;", "").Replace("-", "");

            innings.NameOnScorecard = player.InnerText;
            innings.HowOutText = cells[1].InnerText;

            if (!string.IsNullOrEmpty(runs) && !string.IsNullOrWhiteSpace(runs))
                innings.Runs = int.Parse(runs);
            if (!string.IsNullOrEmpty(balls) && !string.IsNullOrWhiteSpace(balls))
                innings.Balls = int.Parse(balls);
            if (!string.IsNullOrEmpty(mins) && !string.IsNullOrWhiteSpace(mins))
                innings.Minutes = int.Parse(mins);
            if (!string.IsNullOrEmpty(fours) && !string.IsNullOrWhiteSpace(fours))
                innings.Fours = int.Parse(fours);
            if (!string.IsNullOrEmpty(sixes) && !string.IsNullOrWhiteSpace(sixes))
                innings.Sixes = int.Parse(sixes);

            ProcessHowOut(cells[1], innings);

            return innings;
        }

        private static int GetPlayerId(HtmlNode node)
        {
            if (node.NodeType == HtmlNodeType.Element && node.Name == "a")
            {
                string url = node.Attributes["href"].Value;
                Match m = Regex.Match(url, @"/Archive/Players/\d+/\d+/(?'id'\d+)\.html");
                int id = int.Parse(m.Groups["id"].Value);
                return id;
            }

            return 0;
        }

        private static void ProcessHowOut(HtmlNode cell, BatsmanInnings innings)
        {
            List<string> players = new List<string>();
            HtmlNodeCollection playerNodes = cell.SelectNodes("./a");
            if (playerNodes != null && playerNodes.Count > 0)
            {
                players.AddRange(cell.SelectNodes("./a").Select(FindPlayerId));
            }

            IHowOut howOut = HowOutFactory.GetHowOut(GetTrimmedInnerText(cell.FirstChild));
            innings.HowOut = howOut.HowOutType;
            innings.IsInnings = howOut.IsInnings;
            innings.IsOut = howOut.IsOut;
            if (howOut.HasFielder)
                innings.FielderId = howOut.GetFielder(players);
            if (howOut.HasBowler)
                innings.BowlerId = howOut.GetBowler(players);
        }

        private static string FindPlayerId(HtmlNode player)
        {
            int playerId = GetPlayerId(player);
            return PlayerDetails.GenerateId(playerId);
        }


        private void AddBowling(HtmlNode bowlingNode, Innings innings)
        {
            HtmlNodeCollection rows = bowlingNode.SelectNodes(".//tr");
            if (rows == null)
                return;
            if (rows.Count < 2)
            {
                Log.WarnFormat("Only found {0} lines in the bowling HTML. Expected at least 2", rows.Count);
                return;
            }

            innings.Bowling = new List<BowlerInnings>();

            string team = ExtractWhoseBowling(rows[0]);

            for (int i = 1; i < rows.Count; i++)
            {
                BowlerInnings bowler = ParseBowlerInnings(rows[i]);
                bowler.Number = i;
                bowler.Team = team;
                innings.Bowling.Add(bowler);
            }
        }

        private BowlerInnings ParseBowlerInnings(HtmlNode row)
        {
            HtmlNodeCollection cells = row.SelectNodes("./td");
            BowlerInnings innings = new BowlerInnings();
           
            HtmlNode playerNode = cells[0].FirstChild;
            if (playerNode.NodeType == HtmlNodeType.Element && playerNode.Name == "a")
            {
                innings.BowlerId = FindPlayerId(playerNode);
            }

            innings.NameOnScorecard = playerNode.InnerText;

            string oversText = GetTrimmedInnerText(cells[1]);
            if (oversText != "?" && oversText != "-")
            {
                string overs = oversText;
                string balls = null;
                int dot = oversText.LastIndexOf('.');
                if (dot >= 0)
                {
                    overs = oversText.Substring(0, dot);
                    balls = oversText.Substring(dot + 1);
                }
                int completedOvers = int.Parse(overs);
                int extraBalls = string.IsNullOrEmpty(balls) ? 0 : int.Parse(balls);

                innings.Overs = string.IsNullOrEmpty(balls) ? completedOvers.ToString() : string.Format("{0}.{1}", completedOvers, extraBalls);
                innings.BallsBowled = (completedOvers * _match.BallsPerOver) + extraBalls;
            }

            string maidensText = GetTrimmedInnerText(cells[2]);
            int maidens;
            if (int.TryParse(maidensText, out maidens))
                innings.Maidens = maidens;

            string runsText = GetTrimmedInnerText(cells[3]);
            int runs;
            if (int.TryParse(runsText, out runs))
                innings.Runs = runs;

            string wicketsText = GetTrimmedInnerText(cells[4]);
            int wickets;
            if (int.TryParse(wicketsText, out wickets))
                innings.Wickets = wickets;

            string widesText = GetTrimmedInnerText(cells[5]);
            int wides;
            if (int.TryParse(widesText, out wides))
                innings.Wides = wides;

            string noballsText = GetTrimmedInnerText(cells[6]);
            int noballs;
            if (int.TryParse(noballsText, out noballs))
                innings.NoBalls = noballs;

            return innings;
        }

        private static string GetTrimmedInnerText(HtmlNode node)
        {
            if (node == null)
                return string.Empty;

            string text = node.InnerText;
            text = text.Replace("&nbsp;", " ");
            text = text.Trim();
            return text;
        }

        private static string ExtractWhoseBowling(HtmlNode headerRow)
        {
            HtmlNode cell = headerRow.SelectSingleNode("./td[1]");

            return Regex.Replace(cell.InnerText, @"\s+bowling\s*$", "");
        }

/*
        private PlayerDetails ParsePlayer(HtmlNode node)
        {
            if (node.NodeType == HtmlNodeType.Element && node.Name == "a")
            {
                string url = node.Attributes["href"].Value;
                Match m = Regex.Match(url, @"/Archive/Players/\d+/\d+/(?'id'\d+)\.html");
                int id = int.Parse(m.Groups["id"].Value);
                string name = node.InnerText;

                //return _finder.FindPlayer(id, url, name);
            }

            return null;
        }
*/


        private void SetKeeper(string playerId)
        {
            if (_match.SomersetWicketKeeperId == null)
                _match.SomersetWicketKeeperId = playerId;
        }

        private void SetCaptain(string playerId)
        {
            if (_match.SomersetCaptainId == null)
                _match.SomersetCaptainId = playerId;
        }

    }
}
