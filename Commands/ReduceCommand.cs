using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spider.Domain;
using Spider.MapFunctions;
using Spider.Persistence;
using Spider.ReduceFunctions;

namespace Spider.Commands
{
    class ReduceCommand : BaseCommand, ICommand
    {
        public string StartSeason { get; set; }
        public string EndSeason { get; set; }

        
        public void Execute(FileStore dataStore)
        {
            foreach (Season season in GetSeasons(dataStore, StartSeason, EndSeason))
            {
                Log.InfoFormat("Running map functions for season {0}", season.Name);
                ReduceBatting(season, dataStore);
            }
        }


        private void ReduceBatting(Season season, FileStore dataStore)
        {
            List<BattingRecord> records =
                dataStore.Load<List<BattingRecord>>(IndividualBattingMap.GenerateId(season.Name));

            Batting.Reduce(records, dataStore);
        }


    }
}
