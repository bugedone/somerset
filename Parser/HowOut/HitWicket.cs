﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spider.Domain;

namespace Spider.Parser.HowOut
{
    public class HitWicket : IHowOut
    {

        #region IHowOut Members

        public string ScorecardMatch
        {
            get { return "hit wkt b"; }
        }

        public string DisplayName
        {
            get { return "Hit Wicket"; }
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
            get { return "hit wicket b {1}"; }
        }

        public HowOutType HowOutType
        {
            get { return HowOutType.HitWicket; }
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
