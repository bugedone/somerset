using Spider.Persistence;

namespace Spider.Domain
{
    public class Ground
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CricketArchiveUrl { get; set; }
        public string Address { get; set; }
        public string HomeTeam { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public static string GenerateId(int groundId)
        {
            return "ground/" + groundId;
        }


        public static Ground Load(FileStore store, string id)
        {
            return store.Load<Ground>(id);
        }

        public static void Save(FileStore store, Ground ground)
        {
            store.Save(ground, ground.Id);
        }
    }
}
