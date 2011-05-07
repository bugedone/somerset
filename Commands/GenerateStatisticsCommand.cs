using Raven.Client;
using Spider.Domain;
using NHibernate.Criterion;

namespace Spider.Commands
{
    class GenerateStatisticsCommand : BaseCommand, ICommand
    {
        public string StartSeason { get; set; }
        public string EndSeason { get; set; }

        public void Execute(IDocumentStore dataStore)
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
