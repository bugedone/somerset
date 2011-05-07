using System;
using System.Linq;
using Spider.Domain;
using Spider.Parser;
using Spider.Persistence;

namespace Spider.Commands
{
    class ParseCommand : BaseCommand, ICommand
    {
        public string StartSeason { get; set; }

        public string EndSeason { get; set; }


        public void Execute(FileStore dataStore)
        {
            foreach (Season season in GetSeasons(dataStore, StartSeason, EndSeason))
            {
                ParseScorecards(dataStore, season);
            }
        }

        private static void ParseScorecards(FileStore dataStore, Season season)
        {
            CrawlResults crawlResults = GetCrawlResultsForSeason(dataStore, season);
            if (crawlResults == null)
            {
                Log.WarnFormat("Season {0} has not been crawled yet.", season.Name);
                return;
            }

            Log.InfoFormat("Scorecard parsing started at {0} for season {1}", DateTime.Now.ToShortTimeString(), crawlResults.Season);

            var matchRecords = crawlResults.Classifications.SelectMany(m => m.Scorecards);

            foreach (ScorecardDetails md in matchRecords)
            {
                ParseScorecard(dataStore, md);
            }
            
            Log.InfoFormat("Scorecard parsing finished at {0} for season {1}", DateTime.Now.ToShortTimeString(), season.Name);
        }

        private static void ParseScorecard(FileStore dataStore, ScorecardDetails md)
        {
            CricketMatch m = dataStore.Load<CricketMatch>(CricketMatch.GenerateId(md.MatchCode));
            if (m != null)
            {
                Log.InfoFormat("Match {0} ({1}) has already been imported", md.MatchCode, m);
                return;
            }

            Log.InfoFormat("Parsing scorecard for {0}", md);

            string scorecard = dataStore.LoadText(md.GenerateScorecardKey(), "html");
            if (string.IsNullOrEmpty(scorecard))
                return;

            DependencyFinder finder = new DependencyFinder(dataStore);
            ScorecardParser parser = new ScorecardParser(md, finder);
            parser.Parse(scorecard);

            CricketMatch match = parser.Match;

            dataStore.Save(match, match.Id);

            Log.Info(match.ToLongString());
        }


    }
}
