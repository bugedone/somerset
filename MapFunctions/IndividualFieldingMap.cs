using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spider.Domain;

namespace Spider.MapFunctions
{
    class IndividualFieldingMap
    {
        public static string GenerateId(string season)
        {
            return string.Format("map/fielding/{0}", season);
        }


        public static List<FieldingRecord> Run(CricketMatch match)
        {
            var fieldingRecord = from i in match.Innings
                                 from b in i.Batting
                                 where
                                     ((b.HowOut == HowOutType.Caught || b.HowOut == HowOutType.Stumped) &&
                                      !string.IsNullOrEmpty(b.FielderId))
                                 select new FieldingRecord
                                            {
                                                PlayerId = b.FielderId,
                                                Team = i.Bowling[0].Team,
                                                Catches = b.HowOut == HowOutType.Caught ? 1 : 0,
                                                Stumpings = b.HowOut == HowOutType.Stumped ? 1 : 0
                                            };

            return fieldingRecord.ToList();
        }
    }
}
