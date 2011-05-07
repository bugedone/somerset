using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spider.Domain.HowOut
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
