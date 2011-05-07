using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spider.Domain;

namespace Spider.Parser.HowOut
{
    public class AbsentHurt : IHowOut
    {

        #region IHowOut Members

        public string ScorecardMatch
        {
            get { return "absent hurt"; }
        }

        public string DisplayName
        {
            get { return "Absent Hurt"; }
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
            get { return "absent hurt"; }
        }

        public HowOutType HowOutType
        {
            get { return HowOutType.AbsentHurt; }
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
