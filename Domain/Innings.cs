using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spider.Domain
{
    public class Innings
    {
        public int Number { get; set; }
        public string BattingTeam { get; set; }
        public int TeamInningsNumber { get; set; }
        public int? Byes { get; set; }
        public int? LegByes { get; set; }
        public int? Wides { get; set; }
        public int? NoBalls { get; set; }
        public int? Extras { get; set; }
        public int? Total { get; set; }
        public int? Wickets { get; set; }
        public string Overs { get; set; }
        public int? BallsBowled { get; set; }
        public bool Forfeited { get; set; }
        public bool Declared { get; set; }
        public bool AllOut { get; set; }


        public List<FallOfWicket> FallOfWickets { get; set; }
        public List<BatsmanInnings> Batting { get; set; }
        public List<BowlerInnings> Bowling { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} {1} innings", BattingTeam, TeamInningsNumber.ToOrdinal());
            if (Total.HasValue)
            {
                sb.AppendFormat(", {0}", Total);
                if (AllOut)
                    sb.Append(" all out");
                else if (Wickets.HasValue)
                    sb.AppendFormat(" for {0} wickets", Wickets);
                if (Declared)
                    sb.Append(" declared");
            }
            if (!string.IsNullOrEmpty(Overs))
                sb.AppendFormat(", {0} overs", Overs);
            if (Batting.Count > 0)
            {
                string bestBatting = GetBestBatting();
                string bestBowling = GetBestBowling();

                if (!string.IsNullOrEmpty(bestBatting) && !string.IsNullOrEmpty(bestBowling))
                    sb.AppendFormat(" ({0}, {1})", bestBatting, bestBowling);
            }

            if (Forfeited)
                sb.Append(", innings forfeited.");

            return sb.ToString();
        }

        private string GetBestBatting()
        {
            if (Batting == null || Batting.Count == 0)
                return null;

            List<BatsmanInnings> fifties = Batting.Where(i => i.Runs >= 50).OrderByDescending(i => i.Runs).ToList();
            List<BatsmanInnings> scoresUnder50 = Batting.Where(i => i.Runs > 0 && i.Runs < 50).OrderByDescending(i => i.Runs).ToList();

            if (fifties.Count > 0)
                return string.Join(", ", fifties.Select(i => i.ToShortString()));
            if (scoresUnder50.Count > 0)
                return scoresUnder50[0].ToShortString();

            return null;
        }

        private string GetBestBowling()
        {
            if (Bowling == null || Bowling.Count == 0)
                return null;

            List<BowlerInnings> threeFors = Bowling.Where(i => i.Wickets >= 3).OrderByDescending(i => i.Wickets).ThenBy(i => i.Runs).ToList();
            List<BowlerInnings> bestLessThanThree = Bowling.Where(i => i.Wickets < 3).OrderByDescending(i => i.Wickets).ThenBy(i => i.Runs).ToList();

            if (threeFors.Count > 0)
                return string.Join(", ", threeFors.Select(i => i.ToString()));
            if (bestLessThanThree.Count > 0)
                return bestLessThanThree[0].ToString();

            return null;
        }

    }
}
