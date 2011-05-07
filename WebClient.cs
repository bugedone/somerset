using System;
using System.Text;
using System.Net;
using System.IO;
using log4net;
using HtmlAgilityPack;

namespace Spider
{
    class WebClient
    {
        private static readonly ILog Log = LogManager.GetLogger("Spider");



        /// <summary>
        /// Synchronous fetch
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns>String containing web page content</returns>
        public static string FetchWebPageContent(string url)
        {
            DateTime startedAt = DateTime.Now;
            try
            {
                url = MakeAbsoluteUrl(url);
                if (Log.IsDebugEnabled)
                    Log.DebugFormat("Fetching URL [{0}]", url);
                else 
                    Console.Write(".");

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Proxy.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

                return ExtractContent(resp);
            }
            catch (WebException ex)
            {
                Log.ErrorFormat("Error retrieving URL [{0}]\r\n{1}", url, ex);
                return null;
            }
            finally
            {
                TimeSpan elapsed = DateTime.Now.Subtract(startedAt);
                Log.DebugFormat("URL [{0}] downloaded in {1} ms", url, elapsed.TotalMilliseconds.ToString("0.000"));
            }
        }

        public static HtmlNode GetWebContentNode(string url)
        {
            string content = FetchWebPageContent(url);
            if (string.IsNullOrEmpty(content))
                return null;

            return LoadHtml(content);
        }

        private static HtmlNode LoadHtml(string pageContent)
        {
            HtmlDocument doc = new HtmlDocument {OptionFixNestedTags = true};
            doc.LoadHtml(pageContent);

            if (Log.IsDebugEnabled)
            {
                foreach (HtmlParseError error in doc.ParseErrors)
                {
                    Log.WarnFormat("HTML parse error ({0}) {1} at line {2} position {3}", error.Code, error.Reason, error.Line, error.LinePosition);
                }
            }

            HtmlNode contentDiv = doc.GetElementbyId("columnLeft");
            return contentDiv;
        }



        /// <summary>
        /// Extract page content from HTTP response
        /// </summary>
        /// <param name="resp">HTTP response object</param>
        /// <returns>Page content</returns>
        private static string ExtractContent(HttpWebResponse resp)
        {
            string content = null;
            if (resp.StatusCode == HttpStatusCode.OK )
            {
                Stream responseStream = resp.GetResponseStream();
                if (!string.IsNullOrEmpty(resp.CharacterSet) && responseStream != null)
                {
                    Encoding enc = Encoding.GetEncoding(resp.CharacterSet);
                    StreamReader reader = new StreamReader(responseStream, enc);
                    content = reader.ReadToEnd();
                }
            }
            return content;
        }



        public static string MakeAbsoluteUrl(string relativeUrl)
        {
            if (relativeUrl.StartsWith("http"))
                return relativeUrl;

            if (relativeUrl.StartsWith("/"))
                return string.Format("http://www.cricketarchive.com{0}", relativeUrl);

            return string.Format("http://www.cricketarchive.com/Archive/Seasons/{0}", relativeUrl);
        }

    }
}
