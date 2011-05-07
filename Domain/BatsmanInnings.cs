namespace Spider.Domain
{
    public enum HowOutType
    {
        Unknown,
        Absent,
        AbsentHurt,
        AbsentIll,
        DidNotBat,
        NotOut,
        RetiredHurt,
        RetiredIll,
        Bowled,
        Caught,
        LBW,
        RunOut,
        HitWicket,
        ObstructingTheField,
        Stumped,
        HitTheBallTwice,
        RetiredOut,
        RetiredNotOut
    }

    public class BatsmanInnings
    {
        public int Number { get; set; }
        public string PlayerId { get; set; }
        public string Team { get; set; }
        public string NameOnScorecard { get; set; }
        public string HowOutText { get; set; }
        
        public int Runs { get; set; }
        public int Minutes { get; set; }
        public int Balls { get; set; }
        public int Fours { get; set; }
        public int Sixes { get; set; }

        public string FielderId { get; set; }
        public string BowlerId { get; set; }

        public bool IsOut { get; set; }
        public bool IsInnings { get; set; }
        public HowOutType HowOut { get; set; }

        public override string ToString()
        {
            return NameOnScorecard + " " + HowOutText + (IsInnings ? " " + Runs : "");
        }

        public string ToShortString()
        {
            if (!IsInnings)
                return "";

            return NameOnScorecard + " " + Runs + (!IsOut ? "*" : "");
        }
    }
}
