using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spider.Domain;

namespace Spider.Parser.HowOut
{
    public class Bowled : IHowOut
    {

        #region IHowOut Members

        public string ScorecardMatch
        {
            get { return "b"; }
        }

        public string DisplayName
        {
            get { return "Bowled"; }
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
            get { return "b {1}"; }
        }

        public HowOutType HowOutType
        {
            get { return HowOutType.Bowled; }
        }


        public string GetFielder(IList<string> players)
        {
            return null;
        }

        public string GetBowler(IList<string> players)
        {
            return (players.Count >= 1 ? players[0] : null);
        }
        #endregion
    }
}
