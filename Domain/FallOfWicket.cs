namespace Spider.Domain
{
    public class FallOfWicket
    {
        public int Wicket { get; set; }
        public int? Runs { get; set; }
        public string OutBatsmanId { get; set; }
        public string Overs { get; set; }

        public override string ToString()
        {
            return string.Format("{0}-{1}", Wicket, Runs);
        }
    }
}
