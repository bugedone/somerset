using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spider.Domain;

namespace Spider.MapFunctions
{
    class IndividualBattingMap
    {
        public static string GenerateId(string season)
        {
            return string.Format("map/batting/{0}", season);
        }


        public static List<BattingRecord> Run(CricketMatch match)
        {
            var battingRecords = from i in match.Innings
                                 from b in i.Batting
                                 select CreateBattingRecord(b, i.TeamInningsNumber == 1);

            return battingRecords.ToList();
        }


        private static BattingRecord CreateBattingRecord(BatsmanInnings innings, bool firstInnings)
        {
            var record = new BattingRecord
                             {
                                 PlayerId = innings.PlayerId,
                                 Team = innings.Team,
                                 Runs = innings.Runs,
                                 Minutes = innings.Minutes,
                                 Balls = innings.Balls,
                                 Fours = innings.Fours,
                                 Sixes = innings.Sixes,
                                 Matches = firstInnings ? 1 : 0,
                                 Innings = innings.IsInnings ? 1 : 0,
                                 NotOut = innings.IsInnings && !innings.IsOut ? 1 : 0
                             };
            switch (innings.HowOut)
            {
                case HowOutType.Absent:
                case HowOutType.AbsentHurt:
                case HowOutType.AbsentIll:
                    record.Absent = 1;
                    break;
                case HowOutType.Bowled:
                    record.Bowled = 1;
                    break;
                case HowOutType.Caught:
                    record.Caught = 1;
                    break;
                case HowOutType.LBW:
                    record.Lbw = 1;
                    break;
                case HowOutType.HitWicket:
                    record.HitWicket = 1;
                    break;
                case HowOutType.RunOut:
                    record.RunOut = 1;
                    break;
                case HowOutType.Stumped:
                    record.Stumped = 1;
                    break;
                case HowOutType.HitTheBallTwice:
                case HowOutType.ObstructingTheField:
                case HowOutType.Unknown:
                    record.Other = 1;
                    break;
                case HowOutType.RetiredHurt:
                case HowOutType.RetiredIll:
                case HowOutType.RetiredNotOut:
                case HowOutType.RetiredOut:
                    record.Retired = 1;
                    break;
            }


            return record;
        }
    }
}
