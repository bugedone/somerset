using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spider.Domain;

namespace Spider.MapFunctions
{
    class IndividualBowlingMap
    {
        public static string GenerateId(string season)
        {
            return string.Format("map/bowling/{0}", season);
        }


        public static List<BowlingRecord> Run(CricketMatch match)
        {
            var bowlingRecords = from i in match.Innings
                                 where i.Bowling != null
                                 from b in i.Bowling
                                 select CreateBowlingRecord(b);

            return bowlingRecords.ToList();
        }

        private static BowlingRecord CreateBowlingRecord(BowlerInnings innings)
        {
            return new BowlingRecord
                       {
                           PlayerId = innings.BowlerId,
                           Team = innings.Team,
                           BallsBowled = innings.BallsBowled.HasValue ? innings.BallsBowled.Value : 0,
                           Maidens = innings.Maidens.HasValue ? innings.Maidens.Value : 0,
                           Runs = innings.Runs.HasValue ? innings.Runs.Value : 0,
                           Wickets = innings.Wickets.HasValue ? innings.Wickets.Value : 0,
                           NoBalls = innings.NoBalls.HasValue ? innings.NoBalls.Value : 0,
                           Wides = innings.Wides.HasValue ? innings.Wides.Value : 0
                       };
        }
    }
}
