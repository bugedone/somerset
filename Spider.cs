using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Spider.Crawler;
using Spider.Domain;

namespace Spider
{
    class Spider
    {
        private static readonly ILog Log = LogManager.GetLogger("Spider");

        public CrawlResults Recheck(CrawlResults crawlResults)
        {
            Console.Write("Rechecking {0} ", crawlResults.Season);
            var classificationLinks = from c in crawlResults.Classifications
                                      select
                                          new CrawlerLinkDetails
                                              {
                                                  DestinationPageType = PageType.MatchList,
                                                  DestinationUrl = c.Url,
                                                  LinkText = c.Name,
                                                  SourcePageType = PageType.MatchClassification,
                                                  SourcePageUrl = c.LocationIndexUrl
                                              };

            List<MatchClassification> matchClassifications = new List<MatchClassification>();
            var taskQueue = new Queue<Task<MatchClassification>>();

            foreach (var link in classificationLinks)
            {
                CrawlerLinkDetails l = link;
                taskQueue.Enqueue(Task<MatchClassification>.Factory.StartNew(() => CrawlClassification(l, l.LinkText, crawlResults.Season)));
            }

            if (taskQueue.Count == 0)
            {
                Console.WriteLine(" done.");
                Log.InfoFormat("No URLs to check for season {0}", crawlResults.Season);
                return crawlResults;
            }

            Task.Factory.ContinueWhenAll(taskQueue.ToArray(),
                    completedTasks =>
                    {
                        foreach (Task<MatchClassification> task in completedTasks)
                        {
                            if (task.Exception == null)
                                matchClassifications.Add(task.Result);
                            else
                            {
                                Log.Error("Unexpected exception",
                                        task.Exception);
                            }
                        }
                    })
                    .Wait();

            Console.WriteLine(" done.");

            return new CrawlResults
                       {
                           Id = crawlResults.Id, 
                           Season = crawlResults.Season, 
                           Classifications = matchClassifications
                       };
        }



        public CrawlResults Crawl(Season season)
        {
            Console.Write("Crawling " + season.Name + " ");
            CrawlResults results = new CrawlResults { Season = season.Name};

            CrawlerLinkDetails seasonPage = new CrawlerLinkDetails
                                                {
                                                    SourcePageType = PageType.SeasonList,
                                                    SourcePageUrl = "http://cricketarchive.com/Archive/Seasons/index.html",
                                                    DestinationPageType = PageType.LocationList,
                                                    DestinationUrl = season.Url,
                                                    LinkText = season.Name
                                                };

            List<CrawlerLinkDetails> locationLinks = PageCrawler.CrawlLinksPage(seasonPage);

            List<MatchClassification> classifications = new List<MatchClassification>();

            var taskQueue = new Queue<Task<List<MatchClassification>>>();

            foreach (var link in locationLinks)
            {
                CrawlerLinkDetails l = link;
                taskQueue.Enqueue(Task<List<MatchClassification>>.Factory.StartNew(() => CrawlLocation(l, season.Name)));
            }

            Task.Factory.ContinueWhenAll(taskQueue.ToArray(), 
                    completedTasks =>
                        {
                            foreach (Task<List<MatchClassification>> task in completedTasks)
                            {
                                if (task.Exception == null)
                                    classifications.AddRange(task.Result);
                                else
                                {
                                    Log.Error("Unexpected exception",
                                            task.Exception);
                                }
                            }
                        })
                    .Wait();

            results.Classifications = classifications;
            Console.WriteLine(" done.");
            return results;
        }


        private static List<MatchClassification> CrawlLocation(CrawlerLinkDetails locationLink, string season)
        {
            List<MatchClassification> matchClassifications = new List<MatchClassification>();
            var taskQueue = new Queue<Task<MatchClassification>>();

            foreach (var link in PageCrawler.CrawlLinksPage(locationLink))
            {
                CrawlerLinkDetails l = link;
                taskQueue.Enqueue(Task<MatchClassification>.Factory.StartNew(() => CrawlClassification(l, l.LinkText, season)));
            }

            Task.Factory.ContinueWhenAll(taskQueue.ToArray(),
                    completedTasks =>
                    {
                        foreach (Task<MatchClassification> task in completedTasks)
                        {
                            if (task.Exception == null)
                                matchClassifications.Add(task.Result);
                            else
                            {
                                Log.Error("Unexpected exception",
                                        task.Exception);
                            }
                        }
                    })
                    .Wait();

            return matchClassifications;

        }


        private static MatchClassification CrawlClassification(CrawlerLinkDetails classificationLink, string location, string season)
        {
            return new MatchClassification
                       {
                           Location = location,
                           LocationIndexUrl = classificationLink.SourcePageUrl,
                           Name = classificationLink.LinkText,
                           Url = classificationLink.DestinationUrl,
                           Scorecards = PageCrawler.CrawlMatchListPage(classificationLink, season)
                       };
        }


    }



    class CrawlerLinkDetails
    {
        public string DestinationUrl { get; set; }
        public string LinkText { get; set; }
        public string SourcePageUrl { get; set; }
        public PageType SourcePageType { get; set; }
        public PageType DestinationPageType { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1} from {2})", LinkText, DestinationUrl, SourcePageUrl);
        }
    }

    enum PageType
    {
        SeasonList,
        LocationList,
        MatchClassification,
        MatchList
    }

}
