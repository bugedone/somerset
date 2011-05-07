using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spider.Domain;
using Spider.Persistence;

namespace Spider.Commands
{
    class DownloadCommand : BaseCommand, ICommand
    {
        public string StartSeason { get; set; }
        public string EndSeason { get; set; }

        public void Execute(FileStore dataStore)
        {
            foreach (Season season in GetSeasons(dataStore, StartSeason, EndSeason))
            {
                DownloadScorecards(dataStore, season);
            }
        }

        private static void DownloadScorecards(FileStore dataStore, Season season)
        {
            CrawlResults crawlResults = GetCrawlResultsForSeason(dataStore, season);
            if (crawlResults == null)
            {
                Log.WarnFormat("Season {0} has not been crawled yet.", season.Name);
                return;
            }

            Log.InfoFormat("Scorecard download started at {0} for season {1}", DateTime.Now.ToShortTimeString(), crawlResults.Season);

            var matchRecords = crawlResults.Classifications.SelectMany(m => m.Scorecards);

            Queue<Task> tasks = new Queue<Task>();

            foreach (ScorecardDetails md in matchRecords)
            {
                if (md.ScorecardAvailable && !string.IsNullOrEmpty(md.ScorecardUrl))
                {
                    Log.InfoFormat("Downloading scorecard for {0}", md);

                    tasks.Enqueue(DownloadScorecardAsync(md, dataStore));
                }
            }

            Task.WaitAll(tasks.ToArray());

            SaveCrawlerResults(crawlResults, dataStore);

            Log.InfoFormat("Scorecard download finished at {0} for season {1}", DateTime.Now.ToShortTimeString(), crawlResults.Season);
        }

        private static Task DownloadScorecardAsync(ScorecardDetails details, FileStore dataStore)
        {
            Log.InfoFormat("Fetching URL {0}", details.ScorecardUrl);
            return Task.Factory.StartNew(() => WebClient.FetchWebPageContent(details.ScorecardUrl))
                               .ContinueWith(t => SaveScorecard(details, t, dataStore));
        }


        private static void SaveScorecard(ScorecardDetails details, Task<string> task, FileStore dataStore)
        {
            if (task.Exception != null)
            {
                Log.Error(string.Format("Failed to download file from http://www.cricketarchive.com{0}", details.ScorecardUrl),
                          task.Exception);
                return;
            }

            string scorecard = task.Result;
            if (string.IsNullOrEmpty(scorecard))
            {
                Log.WarnFormat("Nothing returned from http://www.cricketarchive.com{0}", details.ScorecardUrl);
                return;
            }

            dataStore.StoreText(scorecard, details.GenerateScorecardKey(), "html");
        }
    }
}
