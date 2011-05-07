using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using Wintellect.PowerCollections;
using HtmlAgilityPack;

namespace Spider.Parser
{
    class SeasonsPageParser
    {
        public OrderedDictionary<string, string> Parse(string pageUrl) 
        {
            OrderedDictionary<string, string> seasons = new OrderedDictionary<string, string>((a, b) => b.CompareTo(a));

            HtmlNode contentDiv = WebClient.GetWebContentNode(pageUrl);
            if (contentDiv == null)
                return seasons;

            HtmlNodeCollection links = contentDiv.SelectNodes("table[1]//a");
            foreach (HtmlNode link in links)
            {
                string season = link.InnerText.Replace('/', '-');
                string url = link.GetAttributeValue("href", null);

                if (!string.IsNullOrEmpty(season) && !string.IsNullOrEmpty(url))
                {
                    seasons.Add(season, url);
                }
            }

            return seasons;
        }
    }
}
