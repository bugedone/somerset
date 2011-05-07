using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using Wintellect.PowerCollections;
using System.IO;
using Spider.Parser;
using Spider.Domain;
using log4net;

namespace Spider.Commands
{
    class BaseCommand
    {
        protected static readonly ILog Log = LogManager.GetLogger("Spider");

        #region Seasons Functions
        protected static string GenerateSeasonFolder(ScorecardDetails details)
        {
            string seasonYear = details.Season.Substring(0, 4);
            int year = int.Parse(seasonYear);
            int decade = (year / 10) * 10;

            string path = string.Format(@"{0}s\{1}", decade, details.Season);
            return path;
        }

        //protected static OrderedDictionary<string, string> GetSeasons()
        //{
        //    SeasonsPageParser parser = new SeasonsPageParser();
        //    return parser.Parse("http://www.cricketarchive.com/Archive/Seasons/index.html");
        //}

        //protected static OrderedDictionary<string, Season> GetSeasons(IDocumentStore dataStore)
        //{
        //    using (IDocumentSession session = dataStore.OpenSession())
        //    {
        //        var seasons = session.Load<AllSeasons>("seasons").Seasons;
        //        return new OrderedDictionary<string, Season>(seasons.Select(s => new KeyValuePair<string, Season>(s.Name, s)));
        //    }

        //}

        //protected static OrderedDictionary<string, string> GetSeasonsFromFolders()
        //{
        //    const string root = @"Cricket\Scorecards\";
        //    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), root);

        //    OrderedDictionary<string, string> seasons = new OrderedDictionary<string, string>();
        //    DirectoryInfo dir = new DirectoryInfo(path);
        //    foreach (var decade in dir.GetDirectories())
        //    {
        //        foreach (var season in decade.GetDirectories())
        //        {
        //            seasons.Add(season.Name, season.FullName);
        //        }
        //    }

        //    return seasons;
        //}

        //// TODO: Change to IEnumerable<Season>
        //// TODO: Don't bother getting from folders since now there's no need for web site crawl
        //protected static IEnumerable<string> GetSeasons(string startAt, string endAt, bool fromFolders)
        //{
        //    OrderedDictionary<string, string> seasons = fromFolders ? GetSeasonsFromFolders() : GetSeasons();

        //    List<string> keys = seasons.Keys.ToList();
        //    keys.Sort();

        //    int startPosition = 0;
        //    int endPosition = keys.Count - 1;

        //    if (!string.IsNullOrEmpty(startAt) && seasons.ContainsKey(startAt))
        //    {
        //        startPosition = keys.IndexOf(startAt);
        //    }

        //    if (!string.IsNullOrEmpty(endAt) && seasons.ContainsKey(endAt))
        //    {
        //        endPosition = keys.IndexOf(endAt);
        //    }

        //    if (startPosition > endPosition)
        //    {
        //        int temp = startPosition;
        //        startPosition = endPosition;
        //        endPosition = temp;
        //    }

        //    for (int i = startPosition; i <= endPosition; i++)
        //    {
        //        yield return keys[i];
        //    }
        //}


        protected static IEnumerable<Season> GetSeasons(IDocumentStore dataStore, string startSeason, string endSeason)
        {
            using (IDocumentSession session = dataStore.OpenSession())
            {
                AllSeasons allSeasons = session.Load<AllSeasons>("seasons");
                if (allSeasons == null)
                {
                    Log.Warn("Seasons page has not been parsed. Please run the 'seasons' command first.");
                    return new Season[0];
                }
                var seasons = allSeasons.Seasons
                                .Where(x => x.Name.CompareTo(startSeason) >= 0 && x.Name.CompareTo(endSeason) <= 0)
                                .OrderBy(x => x.Name);

                return seasons;
            }

        }

        protected static CrawlResults GetCrawlResultsForSeason(IDocumentStore dataStore, Season season)
        {
            using (IDocumentSession session = dataStore.OpenSession())
            {
                return session.Load<CrawlResults>("crawler/" + season.Name);
            }
        }

        #endregion

        protected static void SaveCrawlerResults(CrawlResults results, IDocumentStore dataStore)
        {
            using (IDocumentSession session = dataStore.OpenSession())
            {
                if (string.IsNullOrEmpty(results.Id))
                    results.Id = CrawlResults.GenerateKey(results);

                session.Store(results);
                session.SaveChanges();
            }
        }
    }
}
