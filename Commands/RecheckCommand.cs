using System;
using Spider.Domain;
using Spider.Persistence;

namespace Spider.Commands
{
    class RecheckCommand : BaseCrawlCommand, ICommand
    {
        public string StartSeason { get; set; }
        public string EndSeason { get; set; }

        public void Execute(FileStore dataStore)
        {
            foreach (Season season in GetSeasons(dataStore, StartSeason, EndSeason))
            {
                RecheckSeason(dataStore, season);
            }
        }

        private static void RecheckSeason(FileStore dataStore, Season season)
        {
            CrawlResults existing = GetCrawlResultsForSeason(dataStore, season);
            if (existing == null)
            {
                Log.WarnFormat("Season {0} has not been crawled yet.", season.Name);
                return;
            }

            Spider spider = new Spider();
            CrawlResults recheckResults = spider.Recheck(existing);

            Log.InfoFormat("\n{0}", DumpResults(recheckResults));

            Log.InfoFormat("Crawler finished at {0}.", DateTime.Now.ToShortTimeString());
            SaveCrawlerResults(recheckResults, dataStore);
        }

    }
}
