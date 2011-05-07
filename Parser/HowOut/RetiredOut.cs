using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spider.Domain;

namespace Spider.Parser.HowOut
{
    public class RetiredOut : IHowOut
    {

        #region IHowOut Members

        public string ScorecardMatch
        {
            get { return "retired out"; }
        }

        public string DisplayName
        {
            get { return "Retired Out"; }
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
            get { return "retired out"; }
        }

        public HowOutType HowOutType
        {
            get { return HowOutType.RetiredOut; }
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
