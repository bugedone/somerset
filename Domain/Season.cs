using System.Collections.Generic;

namespace Spider.Domain
{
    public class AllSeasons
    {
        public string Id { get; set; }
        public List<Season> Seasons { get; set; }
    }


    public class Season
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
