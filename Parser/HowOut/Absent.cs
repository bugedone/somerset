using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spider.Domain;

namespace Spider.Parser.HowOut
{
    public class Absent : IHowOut
    {

        #region IHowOut Members

        public string ScorecardMatch
        {
            get { return "absent"; }
        }

        public string DisplayName
        {
            get { return "Absent"; }
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
            get { return "absent"; }
        }

        public HowOutType HowOutType
        {
            get { return HowOutType.Absent; }
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
