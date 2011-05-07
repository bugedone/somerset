using System;
using System.Collections.Generic;


namespace Spider.Domain
{
    public class CrawlResults
    {
        public string Id { get; set; }
        public string Season { get; set;  }
        public List<MatchClassification> Classifications { get; set; }

        
        public static string GenerateKey(CrawlResults crawlResults)
        {
            return "crawler/" + crawlResults.Season;
        }
    }

    public class MatchClassification
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Location { get; set; }
        public string LocationIndexUrl { get; set; }
        public List<ScorecardDetails> Scorecards { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }


    public class ScorecardDetails
    {
        public string MatchCode { get; set; }
        public DateTime Date { get; set; }
        public string Season { get; set; }
        public string ScorecardUrl { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string GroundUrl { get; set; }
        public string GroundName { get; set; }
        public bool ScorecardAvailable { get; set; }
        public string FileName { get; set; }
        public DateTime LastChecked { get; set; }


        public override string ToString()
        {
            return string.Format("{0}: {1} v {2} on {3} at {4}", MatchCode, HomeTeam, AwayTeam, Date.ToString("ddd, dd MMM yyyy"), GroundName);
        }

        public override bool Equals(object obj)
        {
            ScorecardDetails md = obj as ScorecardDetails;
            if (md == null)
                return false;

            if (MatchCode == md.MatchCode && !string.IsNullOrWhiteSpace(MatchCode))
                return true;
            if (!string.IsNullOrWhiteSpace(MatchCode) && !string.IsNullOrWhiteSpace(md.MatchCode))
                return MatchCode == md.MatchCode;

            return Date == md.Date && HomeTeam == md.HomeTeam && AwayTeam == md.AwayTeam;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
