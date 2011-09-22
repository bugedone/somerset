using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spider.Domain;
using Spider.MapFunctions;
using Spider.Persistence;

namespace Spider.Commands
{
    class RunMapCommand : BaseCommand, ICommand
    {
        public string StartSeason { get; set; }
        public string EndSeason { get; set; }


        public void Execute(FileStore dataStore)
        {
            foreach (Season season in GetSeasons(dataStore, StartSeason, EndSeason))
            {
                Log.InfoFormat("Running map functions for season {0}", season.Name);
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

            if (matchRecords.Count() == 0)
            {
                Log.InfoFormat("No match records found for {0}", season.Name);
                return;
            }


            var battingTasks = new Queue<Task<List<BattingRecord>>>();
            var bowlingTasks = new Queue<Task<List<BowlingRecord>>>();
            var fieldingTasks = new Queue<Task<List<FieldingRecord>>>();

            foreach (ScorecardDetails details in matchRecords)
            {
                string id = CricketMatch.GenerateId(details.Season, details.MatchCode);
                CricketMatch match = dataStore.Load<CricketMatch>(id);
                if (match != null)
                {
                    battingTasks.Enqueue(Task<List<BattingRecord>>.Factory.StartNew(() => IndividualBattingMap.Run(match)));
                    bowlingTasks.Enqueue(Task<List<BowlingRecord>>.Factory.StartNew(() => IndividualBowlingMap.Run(match)));
                    fieldingTasks.Enqueue(Task<List<FieldingRecord>>.Factory.StartNew(() => IndividualFieldingMap.Run(match)));
                }
            }

            List<BattingRecord> battingRecords = new List<BattingRecord>();
            List<BowlingRecord> bowlingRecords = new List<BowlingRecord>();
            List<FieldingRecord> fieldingRecords = new List<FieldingRecord>();

            Task[] continuations = new[] {
                Task.Factory.ContinueWhenAll(battingTasks.ToArray(),
                                             completedTasks =>
                                                 {
                                                     foreach (var task in completedTasks)
                                                     {
                                                         if (task.Exception == null)
                                                             battingRecords.AddRange(task.Result);
                                                         else
                                                             Log.Error("Unexpected exception", task.Exception);
                                                     }
                                                 }),
                Task.Factory.ContinueWhenAll(bowlingTasks.ToArray(),
                                             completedTasks =>
                                             {
                                                 foreach (var task in completedTasks)
                                                 {
                                                     if (task.Exception == null)
                                                         bowlingRecords.AddRange(task.Result);
                                                     else
                                                         Log.Error("Unexpected exception", task.Exception);
                                                 }
                                             }),
                Task.Factory.ContinueWhenAll(fieldingTasks.ToArray(),
                                             completedTasks =>
                                             {
                                                 foreach (var task in completedTasks)
                                                 {
                                                     if (task.Exception == null)
                                                         fieldingRecords.AddRange(task.Result);
                                                     else
                                                         Log.Error("Unexpected exception", task.Exception);
                                                 }
                                             })};

            Task.WaitAll(continuations);



            dataStore.Save(battingRecords, IndividualBattingMap.GenerateId(season.Name));
            dataStore.Save(bowlingRecords, IndividualBowlingMap.GenerateId(season.Name));
            dataStore.Save(fieldingRecords, IndividualFieldingMap.GenerateId(season.Name));

            Log.InfoFormat("Scorecard parsing finished at {0} for season {1}", DateTime.Now.ToShortTimeString(), season.Name);
        }


    }
}
