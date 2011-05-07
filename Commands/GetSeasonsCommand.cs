using System.Linq;
using HtmlAgilityPack;
using Raven.Client;
using Spider.Domain;

namespace Spider.Commands
{
    class GetSeasonsCommand : BaseCrawlCommand, ICommand
    {
        private const string SEASONS_PAGE_URL = "http://www.cricketarchive.com/Archive/Seasons/index.html";

        public void Execute(IDocumentStore dataStore)
        {
            HtmlNode contentDiv = WebClient.GetWebContentNode(SEASONS_PAGE_URL);
            if (contentDiv == null)
            {
                Log.Error("Seasons page content not found!");
                return;
            }

            var seasons =
                contentDiv.SelectNodes("table[1]//a")
                           .Select(node => new Season
                                               {
                                                   Name = node.InnerText.Replace('/', '-'), 
                                                   Url = node.GetAttributeValue("href", null)
                                               })
                           .Where(s => s.Name.CompareTo("1875") >= 0)
                           .OrderBy(s => s.Name).ToList();

            using (IDocumentSession session = dataStore.OpenSession())
            {
                AllSeasons allSeasons = session.Load<AllSeasons>("seasons");
                if (allSeasons == null)
                {
                    allSeasons = new AllSeasons { Id = "seasons", Seasons = seasons};
                }
                else
                {
                    allSeasons.Seasons = seasons;
                }

                session.Store(allSeasons);
                session.SaveChanges();
           }
        }
    }
}
