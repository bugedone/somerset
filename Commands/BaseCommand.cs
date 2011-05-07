using System.Collections.Generic;
using System.Linq;
using Spider.Persistence;
using Spider.Domain;
using log4net;

namespace Spider.Commands
{
    class BaseCommand
    {
        protected static readonly ILog Log = LogManager.GetLogger("Spider");

        #region Seasons Functions
        protected static IEnumerable<Season> GetSeasons(FileStore dataStore, string startSeason, string endSeason)
        {

            AllSeasons allSeasons = dataStore.Load<AllSeasons>("seasons");
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

        protected static CrawlResults GetCrawlResultsForSeason(FileStore dataStore, Season season)
        {
            return dataStore.Load<CrawlResults>("crawler/" + season.Name);
        }

        #endregion

        protected static void SaveCrawlerResults(CrawlResults results, FileStore dataStore)
        {
            if (string.IsNullOrEmpty(results.Id))
                results.Id = CrawlResults.GenerateKey(results);

            dataStore.Save(results, results.Id);
        }
    }
}
