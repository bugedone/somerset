using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spider.Domain.HowOut
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

        public PlayerDetails GetFielder(IList<PlayerDetails> players)
        {
            return null;
        }

        public PlayerDetails GetBowler(IList<PlayerDetails> players)
        {
            return null;
        }

        #endregion
    }
}
