using System.Collections.Generic;
using Spider.Domain;
using HtmlAgilityPack;

namespace Spider.Parser
{
    class GroundPageParser
    {
        public static Ground Parse(string url)
        {
            Dictionary<string, string> tokens = Tokenize(url);
            if (tokens.Count > 0)
            {
                Ground ground = new Ground();
                if (tokens.ContainsKey("Ground Name:"))
                    ground.Name = tokens["Ground Name:"];
                if (tokens.ContainsKey("Home Team:"))
                    ground.HomeTeam = tokens["Home Team:"];
                if (tokens.ContainsKey("Address:"))
                    ground.Address = tokens["Address:"];
                if (tokens.ContainsKey("Region:"))
                    ground.Region = tokens["Region:"];
                if (tokens.ContainsKey("Country:"))
                    ground.Country = tokens["Country:"];
                return ground;
            }

            return null;
        }

        private static Dictionary<string, string> Tokenize(string url)
        {
            Dictionary<string, string> tokens = new Dictionary<string, string>();
            HtmlNode contentDiv = WebClient.GetWebContentNode(url);
            if (contentDiv == null)
                return tokens;

            HtmlNodeCollection tableRows = contentDiv.SelectNodes("./table//tr");
            foreach (HtmlNode row in tableRows)
            {
                HtmlNodeCollection cells = row.SelectNodes("./td");
                string key = GetTrimmedInnerText(cells[0]);
                if (cells.Count == 2 && !string.IsNullOrEmpty(key))
                {
                    tokens.Add(key, cells[1].InnerText);
                }
            }

            return tokens;
        }

        private static string GetTrimmedInnerText(HtmlNode node)
        {
            string text = node.InnerText;
            text = text.Replace("&nbsp;", " ");
            text = text.Trim();
            return text;
        }

    }
}
