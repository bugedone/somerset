using Spider.Domain;
using Spider.Persistence;

namespace Spider.Commands
{
    class GenerateStatisticsCommand : BaseCommand, ICommand
    {
        public string StartSeason { get; set; }
        public string EndSeason { get; set; }

        public void Execute(FileStore dataStore)
        {
            foreach (Season season in GetSeasons(dataStore, StartSeason, EndSeason))
            {
                // TODO: Reg stats
                Log.InfoFormat("Regenerating stats for season {0}", season.Name);
                GenerateStatsForSeason(season.Name);
            }
        }


        private void GenerateStatsForSeason(string season)
        {
            // TODO: Start processing data

        }
    }
}
