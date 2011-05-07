using Spider.Domain;
using Spider.Parser;
using log4net;
using Spider.Persistence;

namespace Spider
{
    class DependencyFinder
    {
        private static readonly ILog Log = LogManager.GetLogger("Spider");

        private readonly FileStore _dataStore;

        public DependencyFinder(FileStore dataStore)
        {
            _dataStore = dataStore;
        }

        

        public void CheckGround(int id, string url, string name)
        {
            string groundId = Ground.GenerateId(id);
            Ground ground = Ground.Load(_dataStore, groundId);
            if (ground == null)
            {
                Log.InfoFormat("Ground with ID {0} ({1}) not previously seen. Downloading details...", id, name);

                // OK not already in DB. Fetch and parse
                ground = GroundPageParser.Parse("http://www.cricketarchive.com" + url);
                if (ground != null)
                {
                    ground.Id = groundId;
                    ground.CricketArchiveUrl = url;
                    
                    Ground.Save(_dataStore, ground);
                    Log.InfoFormat("Entry for {0} created.", ground.Name);
                }
            }
        }

        public void CheckPlayer(int id, string url, string nameOnScorecard)
        {
            string playerId = PlayerDetails.GenerateId(id);
            PlayerDetails player = PlayerDetails.Load(_dataStore, playerId);
            if (player == null)
            {
                Log.InfoFormat("Player with ID {0} ({1}) not previously seen. Downloading details...", id, nameOnScorecard);
                player = PlayerPageParser.Parse("http://www.cricketarchive.com" + url);
                if (player != null)
                {
                    player.Id = playerId;
                    player.Name = nameOnScorecard;
                    player.Url = url;

                    PlayerDetails.Save(_dataStore, player);

                    Log.InfoFormat("Entry for {0} created.", player.FullName);
                }
            }
        }
    }
}
