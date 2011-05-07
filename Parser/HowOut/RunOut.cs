using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spider.Domain;

namespace Spider.Parser.HowOut
{
    public class RunOut : IHowOut
    {

        #region IHowOut Members

        public string ScorecardMatch
        {
            get { return "run out"; }
        }

        public string DisplayName
        {
            get { return "Run Out"; }
        }

        public bool IsOut
        {
            get { return true; }
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
            get { return "run out"; }
        }


        public HowOutType HowOutType
        {
            get { return HowOutType.RunOut; }
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
