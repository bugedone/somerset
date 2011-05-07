using System.Linq;
using HtmlAgilityPack;
using Spider.Domain;
using Spider.Persistence;

namespace Spider.Commands
{
    class GetSeasonsCommand : BaseCrawlCommand, ICommand
    {
        private const string SEASONS_PAGE_URL = "http://www.cricketarchive.com/Archive/Seasons/index.html";
        private const string KEY = "seasons";

        public void Execute(FileStore dataStore)
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

            AllSeasons allSeasons = dataStore.Load<AllSeasons>(KEY);
            if (allSeasons == null)
            {
                allSeasons = new AllSeasons { Id = KEY, Seasons = seasons};
            }
            else
            {
                allSeasons.Seasons = seasons;
            }

            dataStore.Save(allSeasons, KEY);
        }
    }
}
