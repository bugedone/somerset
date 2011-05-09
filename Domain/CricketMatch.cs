using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Spider.Domain
{
    public class CricketMatch
    {
        public string Id { get; set; }
        public string MatchCode { get; set; }
        public DateTime StartDate { get; set; }
        public string DaysPlayed { get; set; }
        public string Season { get; set; }
        public string Competition { get; set; }
        public string Description { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public int BallsPerOver { get; set; }
        public string Toss { get; set; }
        public string Length { get; set; }
        public string Result { get; set; }
        public string Points { get; set; }
        public string Umpire1 { get; set; }
        public string Umpire2 { get; set; }
        public string TvUmpire { get; set; }
        public string ManOfTheMatch { get; set; }
        public string Players { get; set; }
        public string CloseOfPlay1 { get; set; }
        public string CloseOfPlay2 { get; set; }
        public string CloseOfPlay3 { get; set; }

        public string VenueGroundId { get; set; }
        public string Venue { get; set; }
        
        public string SomersetCaptainId { get; set; }
        public string SomersetWicketKeeperId { get; set; }
        
        public List<Innings> Innings { get; set; }

        public bool IsFirstClass()
        { 
            return Regex.IsMatch(MatchCode, "^f[0-9]+$"); 
        }
        public bool IsListA()
        {
            return Regex.IsMatch(MatchCode, "^a[0-9]+$");
        }
        public bool IsTwenty20()
        {
            return Regex.IsMatch(MatchCode, "^tt[0-9]+$");
        }


        public override string ToString()
        {
            return string.Format("{0} vs {1} at {2} on {3}", HomeTeam, AwayTeam, Venue, DaysPlayed);
        }

        public string ToLongString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(MatchCode);
            sb.AppendFormat("{0} vs {1} at {2} on {3}", HomeTeam, AwayTeam, Venue, DaysPlayed).AppendLine();
            sb.AppendFormat("{0} ({1} match)", Competition, Length).AppendLine();

            foreach (Innings innings in Innings)
            {
                sb.AppendLine(innings.ToString());
            }

            sb.AppendLine(Result);
            return sb.ToString();
        }


        public static string GenerateId(string matchCode)
        {
            return "match/" + matchCode;
        }
    }
}
