using System.Collections.Generic;
using Spider.Domain;

namespace Spider.Parser.HowOut
{
    public class RetiredHurt : IHowOut
    {

        #region IHowOut Members

        public string ScorecardMatch
        {
            get { return "retired hurt"; }
        }

        public string DisplayName
        {
            get { return "Retired Hurt"; }
        }

        public bool IsOut
        {
            get { return false; }
        }

        public bool IsInnings
        {
            get { return true; }
        }


        public bool HasFielder
        {
            get { return false; }
        }

        public bool HasBowler
        {
            get { return false; }
        }

        public string RenderPattern
        {
            get { return "retired hurt"; }
        }

        public HowOutType HowOutType
        {
            get { return HowOutType.RetiredHurt; }
        }

        public string GetFielder(IList<string> players)
        {
            return null;
        }

        public string GetBowler(IList<string> players)
        {
            return null;
        }

        #endregion
    }
}
