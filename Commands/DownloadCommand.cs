using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Raven.Client;
using Spider.Domain;
using System.IO;

namespace Spider.Commands
{
    class DownloadCommand : BaseCommand, ICommand
    {
        public string StartSeason { get; set; }
        public string EndSeason { get; set; }

        public void Execute(IDocumentStore dataStore)
        {
            foreach (Season season in GetSeasons(dataStore, StartSeason, EndSeason))
            {
                DownloadScorecards(dataStore, season);
            }
        }

        private static void DownloadScorecards(IDocumentStore dataStore, Season season)
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
                if (string.IsNullOrEmpty(md.FileName) && md.ScorecardAvailable && !string.IsNullOrEmpty(md.ScorecardUrl))
                {
                    Log.InfoFormat("Downloading scorecard for {0}", md);

                    tasks.Enqueue(DownloadScorecardAsync(md));
                }
            }

            Task.WaitAll(tasks.ToArray());

            SaveCrawlerResults(crawlResults, dataStore);

            Log.InfoFormat("Scorecard download finished at {0} for season {1}", DateTime.Now.ToShortTimeString(), crawlResults.Season);
        }

        private static Task DownloadScorecardAsync(ScorecardDetails details)
        {
            Log.InfoFormat("Fetching URL {0}", details.ScorecardUrl);
            return Task.Factory.StartNew(() => WebClient.FetchWebPageContent(details.ScorecardUrl))
                               .ContinueWith(t => SaveScorecard(details, t));
        }


        private static void SaveScorecard(ScorecardDetails details, Task<string> task)
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
            
            string fileName = GenerateFileName(details);

            Log.InfoFormat("Saving file {0}", fileName);

            TextWriter writer = new StreamWriter(File.OpenWrite(fileName));
            writer.Write(scorecard);
            writer.Close();

            details.FileName = fileName;
        }

        private static string GenerateFileName(ScorecardDetails details)
        {
            string path = GeneratePath(details);
            return Path.Combine(path, details.MatchCode + ".html");
        }

        private static string GeneratePath(ScorecardDetails details)
        {
            const string root = @"Cricket\Scorecards\";

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), root, GenerateSeasonFolder(details));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

    }
}
