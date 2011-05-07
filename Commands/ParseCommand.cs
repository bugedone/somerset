using System;
using System.Linq;
using Raven.Client;
using Spider.Domain;
using System.IO;
using Spider.Parser;

namespace Spider.Commands
{
    class ParseCommand : BaseCommand, ICommand
    {
        public string StartSeason { get; set; }

        public string EndSeason { get; set; }


        public void Execute(IDocumentStore dataStore)
        {
            foreach (Season season in GetSeasons(dataStore, StartSeason, EndSeason))
            {
                ParseScorecards(dataStore, season);
            }
        }

        private static string LoadScorecardFromFile(ScorecardDetails details)
        {
            if (!File.Exists(details.FileName))
            {
                Log.WarnFormat("Cannot parse match {0}, file {1} not found.", details, details.FileName);
                return null;
            }

            using (TextReader reader = File.OpenText(details.FileName))
            {
                return reader.ReadToEnd();
            }
        }


        private static void ParseScorecards(IDocumentStore dataStore, Season season)
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

        private static void ParseScorecard(IDocumentStore dataStore, ScorecardDetails md)
        {
            if (string.IsNullOrEmpty(md.FileName))
            {
                Log.InfoFormat("Scorecard for {0} has not been downloaded yet.", md.MatchCode);
                return;
            }


            using (IDocumentSession session = dataStore.OpenSession())
            {
                CricketMatch m = session.Load<CricketMatch>(CricketMatch.GenerateId(md.MatchCode));
                if (m != null)
                {
                    Log.InfoFormat("Match {0} ({1}) has already been imported", md.MatchCode, m);
                    return;
                }
            }

            Log.InfoFormat("Parsing scorecard for {0}", md);

            string scorecard = LoadScorecardFromFile(md);
            if (string.IsNullOrEmpty(scorecard))
                return;

            DependencyFinder finder = new DependencyFinder(dataStore);
            ScorecardParser parser = new ScorecardParser(md, finder);
            parser.Parse(scorecard);

            CricketMatch match = parser.Match;

            using (IDocumentSession session = dataStore.OpenSession())
            {
                session.Store(match);
                session.SaveChanges();
            }

            Log.Info(match.ToLongString());
        }


    }
}
