using System;
using Spider.Persistence;
using Spider.Domain;

namespace Spider.Commands
{
    class CrawlCommand : BaseCrawlCommand, ICommand
    {

        public string StartSeason { get; set; }
        public string EndSeason { get; set; }

        public void Execute(FileStore dataStore)
        {
            foreach (Season season in GetSeasons(dataStore, StartSeason, EndSeason))
            {
                Log.DebugFormat("Crawling Season {0} at {1}", season.Name, season.Url);

                CrawlResults results = RunCrawler(season);
                SaveCrawlerResults(results, dataStore);
            }
        }


        private static CrawlResults RunCrawler(Season season)
        {
            Log.InfoFormat("Crawler started at {0} for season {1}", DateTime.Now.ToShortTimeString(), season.Name);

            Spider crawler = new Spider();
            CrawlResults results = crawler.Crawl(season);
            Log.InfoFormat("\n{0}", DumpResults(results));

            Log.InfoFormat("Crawler finished at {0}.", DateTime.Now.ToShortTimeString());

            return results;
        }

    }
}
