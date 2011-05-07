namespace Spider.Domain
{
    public class PlayerAward
    {
        public string Season { get; set; }

        public string Award { get; set; }


        public override string ToString()
        {
            return string.Format("{0} {1}", Award, Season);
        }
    }
}
