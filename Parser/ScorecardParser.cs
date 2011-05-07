using System.Collections.Generic;
using System.Text.RegularExpressions;
using log4net;
using Spider.Domain;
using HtmlAgilityPack;

namespace Spider.Parser
{
    class ScorecardParser
    {
        private static readonly ILog Log = LogManager.GetLogger("Spider");
        private readonly CricketMatch _match;
        private readonly DependencyFinder _finder;


        public ScorecardParser(ScorecardDetails details, DependencyFinder finder)
        {
            _finder = finder;
            _match = new CricketMatch
                         {
                             Id = CricketMatch.GenerateId(details.MatchCode),
                             MatchCode = details.MatchCode,
                             HomeTeam = details.HomeTeam,
                             AwayTeam = details.AwayTeam,
                             Season = details.Season,
                             StartDate = details.Date,
                             Players = "11 per side",
                             Innings = new List<Innings>()
                         };
        }

        public void Parse(string scorecardHtml)
        {
            HtmlNode contentDiv = LoadHtml(scorecardHtml);
            HtmlNodeCollection tables = contentDiv.SelectNodes("./table");

            if (tables.Count == 0)
            {
                Log.Warn("Invalid input text. This does not appear to be a CricketArchive scorecard page");
                Log.Debug(scorecardHtml);
                return;
            }

            // Match #1 is the match info panel
            MatchInfoParser mip = new MatchInfoParser(_match, _finder);
            mip.Parse(tables[0]);
            
            if (tables.Count < 3)
                return;

            // Ensure all the players playing are in the data store
            CheckPlayersForInnings(tables[1]);
            CheckPlayersForInnings(tables[3]);

            InningsParser ip = new InningsParser(_match);

            // Match #2 and 3 are the batting and bowling respectively for the first innings

            _match.Innings.Add(ip.Parse(tables[1], tables[2]));

            // Match #4 and 5 are the batting and bowling respectively for the second innings
            if (tables.Count < 5)
                return;
                
            _match.Innings.Add(ip.Parse(tables[3], tables[4]));

            // Match #6 and 7 are the batting and bowling respectively for the third innings
            if (tables.Count < 7)
                return;

            _match.Innings.Add(ip.Parse(tables[5], tables[6]));

            // Match #8 and 9 are the batting and bowling respectively for the fourth innings
            if (tables.Count < 9)
                return;
                
            _match.Innings.Add(ip.Parse(tables[7], tables[8]));
        }

        public CricketMatch Match { get { return _match; } }


        private void CheckPlayersForInnings(HtmlNode inningsTable)
        {
            HtmlNodeCollection rows = inningsTable.SelectNodes(".//tr");

            int i = 1;
            while (i < rows.Count)
            {
                HtmlNode cell = rows[i].SelectSingleNode("./td[1]");
                HtmlNode playerNode = cell.SelectSingleNode("./a");
                if (playerNode == null && (cell.InnerText == "Extras" || cell.InnerText == "Total" || cell.InnerText == "Innings Forfeited"))
                    break;
                if (playerNode != null)
                    CheckPlayer(playerNode);
                i++;
            }
        }



        private void CheckPlayer(HtmlNode node)
        {
            if (node.NodeType == HtmlNodeType.Element && node.Name == "a")
            {
                string url = node.Attributes["href"].Value;
                Match m = Regex.Match(url, @"/Archive/Players/\d+/\d+/(?'id'\d+)\.html");
                int id = int.Parse(m.Groups["id"].Value);
                string name = node.InnerText;

                _finder.CheckPlayer(id, url, name);
            }
        }

        private static HtmlNode LoadHtml(string pageContent)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.OptionFixNestedTags = true;
            doc.LoadHtml(pageContent);

            if (Log.IsDebugEnabled)
            {
                foreach (HtmlParseError error in doc.ParseErrors)
                {
                    Log.WarnFormat("HTML parse error ({0}) {1} at line {2} position {3}", error.Code, error.Reason, error.Line, error.LinePosition);
                }
            }

            HtmlNode contentDiv = doc.GetElementbyId("columnLeft");
            return contentDiv;
        }

    }
}
