using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Raven.Client;
using Spider.Domain;
using System.Text.RegularExpressions;
using System.IO;

namespace Spider.Commands
{
    class BaseCrawlCommand : BaseCommand
    {
        
        protected static void ProcessSpiderResults(string season, List<ScorecardDetails> matches)
        {
            Log.InfoFormat("\n{0}", DumpResults(season, matches));
            //SaveSpiderResultsBySeason(season, matches);
        }

        protected static string DumpResults(string season, List<ScorecardDetails> matches)
        {
            if (matches.Count == 0)
                return "No Somerset matches found in " + season;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(new string('-', 120));
            string title = "Somerset matches in " + season;
            sb.Append("| ").Append(title.PadRight(116)).AppendLine(" |");
            sb.AppendLine(new string('-', 120));
            foreach (ScorecardDetails match in matches)
            {
                sb.Append("| ").Append(FormatValue(match, 116)).AppendLine(" |");
            }
            sb.AppendLine(new string('-', 120));

            return sb.ToString();
        }

        protected static string DumpResults(CrawlResults results)
        {
            if (results.Classifications.Count == 0)
                return "No Somerset matches found in " + results.Season;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(new string('-', 120));
            string title = "Somerset matches in " + results.Season;
            sb.Append("| ").Append(title.PadRight(116)).AppendLine(" |");
            sb.AppendLine(new string('-', 120));

            var matches = from c in results.Classifications
                          from sc in c.Scorecards
                          select sc;

            foreach (ScorecardDetails match in matches)
            {
                sb.Append("| ").Append(FormatValue(match, 116)).AppendLine(" |");
            }
            sb.AppendLine(new string('-', 120));

            return sb.ToString();
        }



        protected static string FormatValue(ScorecardDetails value, int length)
        {
            string asString = value.ToString();
            asString = Regex.Replace(asString, "\\s+", " ");
            if (asString.Length <= length)
                return asString.PadRight(length);
            return asString.Substring(0, length);
        }
    }
}
