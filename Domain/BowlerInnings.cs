namespace Spider.Domain
{
    public class BowlerInnings
    {
        public string Team { get; set; }
        public string BowlerId { get; set; }
        public int Number { get; set; }
        public string NameOnScorecard { get; set; }
        public string Overs { get; set; }
        public int? BallsBowled { get; set; }
        public int? Maidens { get; set; }
        public int? Runs { get; set; }
        public int? Wickets { get; set; }
        public int? Wides { get; set; }
        public int? NoBalls { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}-{2}", NameOnScorecard, Wickets, Runs);
        }
    }
}
