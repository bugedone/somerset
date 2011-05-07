using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spider.Domain.HowOut
{
    public class StumpedSub : IHowOut
    {

        #region IHowOut Members

        public string ScorecardMatch
        {
            get { return "st sub b"; }
        }

        public string DisplayName
        {
            get { return "Stumped"; }
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
            get { return true; }
        }

        public bool HasBowler
        {
            get { return true; }
        }

        public string RenderPattern
        {
            get { return "st sub b {1}"; }
        }


        public PlayerDetails GetFielder(IList<PlayerDetails> players)
        {
            return null;
        }

        public PlayerDetails GetBowler(IList<PlayerDetails> players)
        {
            return (players.Count >= 1 ? players[0] : null);
        }
        #endregion
    }
}
