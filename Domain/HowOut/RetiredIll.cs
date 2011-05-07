using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spider.Domain.HowOut
{
    public class RetiredIll : IHowOut
    {

        #region IHowOut Members

        public string ScorecardMatch
        {
            get { return "retired ill"; }
        }

        public string DisplayName
        {
            get { return "Retired Ill"; }
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
            get { return "retired ill"; }
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
