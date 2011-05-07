using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Raven.Client;

namespace Spider.Domain
{
    public class PlayerDetails
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string FullName { get; set; }
        public string KnownAs { get; set; }
        public string Born { get; set; }
        public string Died { get; set; }
        public string Batting { get; set; }
        public string Bowling { get; set; }
        public bool WicketKeeper { get; set; }
        public int? SomersetCap { get; set; }
        public int? Benefit { get; set; }
        public List<PlayerAward> Awards { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Id, FullName);
        }

        public static List<PlayerDetails> ParsePlayers(string player)
        {
            const string regex = @"<a\shref=""(?'url'/Archive/Players/\d+/\d+/(?'id'\d+)\.html)"">(?'name'[^<]+)</a>";
            MatchCollection allMatches = Regex.Matches(player, regex, RegexOptions.Singleline);
            var players = from Match m in allMatches
                          select new PlayerDetails
                                     {
                                         Id = GenerateId(int.Parse(m.Groups["id"].Value)),
                                         Name = m.Groups["name"].Value,
                                         Url = m.Groups["url"].Value
                                     };
            return players.ToList();
        }

        public static PlayerDetails Sub = new PlayerDetails { Id = "player/sub", Name = "sub" };

        public static string GenerateId(int id)
        {
            return "player/" + id;
        }

        public static PlayerDetails Load(IDocumentStore store, string id)
        {
            using (IDocumentSession session = store.OpenSession())
            {
                return session.Load<PlayerDetails>(id);
            }
        }

        public static void Save(IDocumentStore store, PlayerDetails player)
        {
            using (IDocumentSession session = store.OpenSession())
            {
                session.Store(player);
                session.SaveChanges();
            }
        }
    }
}
