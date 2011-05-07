using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spider.Domain;

namespace Spider.Parser.HowOut
{
    public class CaughtSubWKUnknown : IHowOut
    {

        #region IHowOut Members

        public string ScorecardMatch
        {
            get { return "c +sub b"; }
        }

        public string DisplayName
        {
            get { return "Caught"; }
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
            get { return true; }
        }

        public string RenderPattern
        {
            get { return "c +sub b {1}"; }
        }

        public HowOutType HowOutType
        {
            get { return HowOutType.Caught; }
        }


        public string GetFielder(IList<string> players)
        {
            return null;
        }

        public string GetBowler(IList<string> players)
        {
            return (players.Count >= 2 ? players[1] : null);
        }
        #endregion
    }
}
