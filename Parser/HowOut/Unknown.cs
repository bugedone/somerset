using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spider.Domain;

namespace Spider.Parser.HowOut
{
    public class Unknown : IHowOut
    {

        #region IHowOut Members

        public string ScorecardMatch
        {
            get { return "[dismissal not known]"; }
        }

        public string DisplayName
        {
            get { return "Unknown"; }
        }

        public bool IsOut
        {
            get { return false; }
        }

        public bool IsInnings
        {
            get { return false; }
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
            get { return "[dismissal not known]"; }
        }

        public string GetFielder(IList<string> players)
        {
            return null;
        }

        public string GetBowler(IList<string> players)
        {
            return null;
        }

        public HowOutType HowOutType
        {
            get { return HowOutType.Unknown; }
        }

        #endregion
    }
}
