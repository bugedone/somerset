using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spider.Domain;

namespace Spider.Parser.HowOut
{
    public class HitTheBallTwice : IHowOut
    {

        #region IHowOut Members

        public string ScorecardMatch
        {
            get { return "hit the ball twice"; }
        }

        public string DisplayName
        {
            get { return "Hit The Ball Twice"; }
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
            get { return "hit the ball twice"; }
        }

        public HowOutType HowOutType
        {
            get { return HowOutType.HitTheBallTwice; }
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
