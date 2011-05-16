using System;
using System.Linq;
using Spider.Domain;
using Spider.Persistence;

namespace Spider.Commands
{
    class GenerateStatisticsCommand : BaseCommand, ICommand
    {
        public string StartSeason { get; set; }
        public string EndSeason { get; set; }
        public string Statistic { get; set; }


        public void Execute(FileStore dataStore)
        {
            foreach (Season season in GetSeasons(dataStore, StartSeason, EndSeason))
            {
                // TODO: Reg stats
                Log.InfoFormat("Regenerating stat {0} for season {1}", Statistic, season.Name);
                RunMapFunctions(dataStore, season);
            }
        }


        private void RunMapFunctions(FileStore dataStore, Season season)
        {
            CrawlResults crawlResults = GetCrawlResultsForSeason(dataStore, season);
            if (crawlResults == null)
            {
                Log.WarnFormat("Season {0} has not been crawled yet.", season.Name);
                return;
            }

            Log.InfoFormat("Map started at {0} for season {1}", DateTime.Now.ToShortTimeString(), crawlResults.Season);

            var matchRecords = crawlResults.Classifications.SelectMany(m => m.Scorecards);

            foreach (ScorecardDetails details in matchRecords)
            {
                string id = CricketMatch.GenerateId(details.Season, details.MatchCode);
                CricketMatch match = dataStore.Load<CricketMatch>(id);
            }

            Log.InfoFormat("Scorecard parsing finished at {0} for season {1}", DateTime.Now.ToShortTimeString(), season.Name);
        }


        private static void ParseScorecard(FileStore dataStore, ScorecardDetails md)
        {
            //CricketMatch m = dataStore.Load<CricketMatch>(CricketMatch.GenerateId(md.Season, md.MatchCode));
            //if (m != null)
            //{
            //    Log.InfoFormat("Match {0} ({1}) has already been imported", md.MatchCode, m);
            //    return;
            //}

            //Log.InfoFormat("Parsing scorecard for {0}", md);

            //string scorecard = dataStore.LoadText(md.GenerateScorecardKey(), "html");
            //if (string.IsNullOrEmpty(scorecard))
            //    return;

            //DependencyFinder finder = new DependencyFinder(dataStore);
            //ScorecardParser parser = new ScorecardParser(md, finder);
            //parser.Parse(scorecard);

            //CricketMatch match = parser.Match;

            //dataStore.Save(match, match.Id);

            //Log.Info(match.ToLongString());
        }

    }
}
