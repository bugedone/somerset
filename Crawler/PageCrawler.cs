using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using log4net;
using Spider.Domain;

namespace Spider.Crawler
{
    class PageCrawler
    {
        private static readonly ILog Logger = LogManager.GetLogger("Spider");


        public static List<CrawlerLinkDetails> CrawlLinksPage(CrawlerLinkDetails linkDetails)
        {
            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("Crawling {0} ...", linkDetails.DestinationUrl);
            HtmlNode contentDiv = WebClient.GetWebContentNode(linkDetails.DestinationUrl);
            if (contentDiv == null)
                return new List<CrawlerLinkDetails>();

            var links = from a in contentDiv.SelectNodes(".//a")
                        where ShouldFollowLink(a.GetAttributeValue("href", null))
                        select
                            new CrawlerLinkDetails
                            {
                                DestinationPageType = GetNextPageType(linkDetails.DestinationPageType),
                                DestinationUrl = a.GetAttributeValue("href", null),
                                LinkText = a.InnerText,
                                SourcePageType = linkDetails.DestinationPageType,
                                SourcePageUrl = linkDetails.DestinationUrl
                            };

            return links.ToList();
        }



        private static PageType GetNextPageType(PageType thisPageType)
        {
            switch (thisPageType)
            {
                case PageType.SeasonList:
                    return PageType.LocationList;
                case PageType.LocationList:
                    return PageType.MatchClassification;
                default: // PageType.MatchClassification:
                    return PageType.MatchList;
            }
        }

        private static bool ShouldFollowLink(string url)
        {
            return !url.StartsWith("Seasonal_Averages/") && !url.StartsWith("Lists/") && !url.StartsWith("Tours_");
        }


        

        public static List<ScorecardDetails> CrawlMatchListPage(CrawlerLinkDetails linkDetails, string season)
        {
            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("Crawling {0} ...", linkDetails.DestinationUrl);
            HtmlNode contentDiv = WebClient.GetWebContentNode(linkDetails.DestinationUrl);
            if (contentDiv == null)
                return new List<ScorecardDetails>();

            var scorecards = from row in contentDiv.SelectNodes("./table//tr")
                             let cells = row.SelectNodes("td")
                             where cells != null && cells.Count == 7
                             select GetDetails(cells, season);

            return scorecards.Where(x => x != null).ToList();
        }

        private static ScorecardDetails GetDetails(HtmlNodeCollection cells, string season)
        {
            string scorecardUrl;
            string homeTeam;
            string awayTeam;
            string groundUrl;
            string groundName;

            DateTime date;
            DateTime.TryParse(cells[1].InnerText.Trim(), out date);

            ParseScorecardLink(cells[4].FirstChild, out scorecardUrl, out homeTeam, out awayTeam);
            if (!IsOfInterest(homeTeam, awayTeam))
                return null;

            ParseGroundLink(cells[5].FirstChild, out groundUrl, out groundName);

            string matchCode = cells[6].InnerText;

            return new ScorecardDetails
                       {
                           Season = season,
                           MatchCode = matchCode,
                           HomeTeam = homeTeam,
                           AwayTeam = awayTeam,
                           GroundName = groundName,
                           GroundUrl = groundUrl,
                           Date = date,
                           LastChecked = DateTime.Now,
                           ScorecardUrl = scorecardUrl,
                           ScorecardAvailable = !string.IsNullOrEmpty(scorecardUrl),
                       };
        }

        private static bool IsOfInterest(string homeTeam, string awayTeam)
        {
            return homeTeam == "Somerset" || awayTeam == "Somerset";
        }


        private static void ParseScorecardLink(HtmlNode node, out string url, out string homeTeam, out string awayTeam)
        {
            url = null;
            homeTeam = null;
            awayTeam = null;

            if (node.NodeType == HtmlNodeType.Element && node.Name == "a")
                url = node.GetAttributeValue("href", null);

            Match m = Regex.Match(node.InnerText, @"(?'homeTeam'[^<]+?)\s+v\s+(?'awayTeam'[^<]+)");
            if (m.Length == 0)
            {
                Logger.WarnFormat("Failed to parse teams out of '{0}'", node.InnerText);
                return;
            }

            homeTeam = m.Groups["homeTeam"].Value.Trim();
            awayTeam = m.Groups["awayTeam"].Value.Trim();
        }

        private static void ParseGroundLink(HtmlNode node, out string url, out string name)
        {
            url = null;

            if (node.NodeType == HtmlNodeType.Element && node.Name == "a")
                url = node.GetAttributeValue("href", null);

            name = node.InnerText;
        }



    }
}
