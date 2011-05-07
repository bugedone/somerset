using System.Text.RegularExpressions;
using Spider.Domain;
using log4net;
using HtmlAgilityPack;

namespace Spider.Parser
{
    class MatchInfoParser
    {
        private static readonly ILog Log = LogManager.GetLogger("Spider");
        private readonly CricketMatch _match;
        private readonly DependencyFinder _finder;

        public MatchInfoParser(CricketMatch match, DependencyFinder finder)
        {
            _match = match;
            _finder = finder;
        }


        public void Parse(HtmlNode matchInfoTable)
        {
            HtmlNodeCollection rows = matchInfoTable.SelectNodes(".//tr");

            // Parse out the competition HTML to extract match type
            ParseCompetition(rows[1].SelectNodes("./td")[1].InnerText);

            _match.BallsPerOver = 6;
            string lastName = null;

            for (int i = 2; i < rows.Count; i++)
            {
                HtmlNodeCollection cells = rows[i].SelectNodes("./td");
                string name = cells[0].InnerText;
                string value = cells[1].InnerText;
                string html = cells[1].InnerHtml;

                if (name == "Venue")
                    ParseVenue(html);
                else if (name == "Balls per over")
                    _match.BallsPerOver = int.Parse(value);
                else if (name == "Toss")
                    _match.Toss = value;
                else if (name == "Result")
                    _match.Result = value;
                else if (name == "Points")
                    _match.Points = value;
                else if (name == "Umpires")
                    ParseUmpires(value);
                else if (name == "TV umpire" || name == "Third umpire")
                    _match.TvUmpire = value;
                else if (name.StartsWith("Scorer"))
                {}
                else if (name == "Players")
                    _match.Players = value;
                else if (name == "Man of the Match")
                    _match.ManOfTheMatch = value;
                else if (name.StartsWith("Close of play day 1"))
                    _match.CloseOfPlay1 = value;
                else if (name.StartsWith("Close of play day 2"))
                    _match.CloseOfPlay2 = value;
                else if (name.StartsWith("Close of play day 3"))
                    _match.CloseOfPlay3 = value;
                else if (string.IsNullOrEmpty(name) && lastName == "Points")
                    _match.Points += "; " + value;
                else
                    Log.InfoFormat("{0}: ({1})", name, value);

                lastName = name;
            }

        }


        private void ParseCompetition(string competition)
        {
            const string regex = @"(?'matchDescription'(?'Competition'[\w\s':-]+)\s\d{4}.*)";
            Match m = Regex.Match(competition, regex, RegexOptions.Singleline);

            string temp = m.Groups["Competition"].Value.Trim();
            if (temp.StartsWith("Other First") || temp.StartsWith("County Match"))
                _match.Competition = "Other First-Class";
            else if (temp.StartsWith("Other"))
                _match.Competition = "Other Match";
            else if (temp.StartsWith("University"))
                _match.Competition = "University Match";
            else if (temp.EndsWith("in England") || temp.EndsWith("in British Isles") || temp.StartsWith("Somerset in"))
                _match.Competition = "Tour Match";
            else if (temp.EndsWith("County Championship") || temp.Contains("CricInfo Championship"))
                _match.Competition = "County Championship";
            else if (temp.EndsWith("League"))
                _match.Competition = "One-Day League";
            else if (temp.StartsWith("Clydesdale Bank 40"))
                _match.Competition = "One-Day League";
            else if (temp.StartsWith("Benson and Hedges"))
                _match.Competition = "Benson and Hedges Cup";
            else if (_match.MatchCode.StartsWith("a"))
            {
                if (temp.EndsWith("Cup") || temp.EndsWith("Trophy"))
                    _match.Competition = "Knock-Out Cup";
                else
                    _match.Competition = "Other List A";
            }
            else if (_match.MatchCode.StartsWith("tt"))
            {
                _match.Competition = temp.StartsWith("Twenty20") ? "Twenty20 Cup" : "Other Twenty20";
            }
            else if (_match.MatchCode.StartsWith("misc"))
                _match.Competition = "Other Match";
            else
            {
                Log.WarnFormat("Couldn't work out competition type: '{0}'", temp);
            }

            _match.Description = m.Groups["matchDescription"].Value;
        }

        private void ParseVenue(string venue)
        {
            const string regex = @"<a\shref=""(?'groundUrl'/Archive/Grounds/\d+/(?'groundId'\d+)\.html)"">(?'groundName'[^<]+)</a>\s+on\s+(?'dates'[\s\w,]+)\s+\((?'length'\d+-(day|over))\s+(single\sinnings\s)?match\)";
            Match m = Regex.Match(venue, regex, RegexOptions.Singleline);

            int id = int.Parse(m.Groups["groundId"].Value);
            string name = m.Groups["groundName"].Value;
            string url = m.Groups["groundUrl"].Value;

            _finder.CheckGround(id, url, name);
            _match.Venue = name;
            _match.VenueGroundId = Ground.GenerateId(id);
            _match.DaysPlayed = m.Groups["dates"].Value;
            _match.Length = m.Groups["length"].Value;
        }

        private void ParseUmpires(string umpires)
        {
            string[] names = Regex.Split(umpires, @"\s*,\s*");
            if (names.Length > 0)
                _match.Umpire1 = names[0];
            if (names.Length > 1)
                _match.Umpire2 = names[1];
        }

    }
}
