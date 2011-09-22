using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spider.Domain;
using Spider.Persistence;
using Wintellect.PowerCollections;

namespace Spider.ReduceFunctions
{
    class Batting
    {


        public static void Reduce(List<BattingRecord> records, FileStore dataStore)
        {
            var grouped = from r in records
                          where r.Team == "Somerset"
                          group r by r.PlayerId into g
                          select new BattingRecord
                                     {
                                         PlayerId = g.Key, 
                                         Team = "Somerset", 
                                         Matches = g.Sum(r => r.Matches),
                                         Innings = g.Sum(r => r.Innings),
                                         NotOut = g.Sum(r => r.NotOut),
                                         Highest = g.Max(r => r.Runs),
                                         Centuries = g.Where(r => r.Runs >= 100).Count(),
                                         Fifties = g.Where(r => r.Runs >= 50 && r.Runs < 100).Count(),
                                         Ducks = g.Where(r => r.NotOut == 0 && r.Runs == 0 && r.Innings == 1).Count(),
                                         Runs =  g.Sum(r => r.Runs),
                                         Balls = g.Sum(r => r.Balls),
                                         Average = g.Sum(r => r.Runs) / (float)(g.Sum(r => r.Innings) - g.Sum(r => r.NotOut))
                                     };


            Console.WriteLine("Name                  M  I NO Runs  HS   Ave 100 50  0");

            foreach (var rec in grouped)
            {
                PlayerDetails player = dataStore.Load<PlayerDetails>(rec.PlayerId);
                Console.WriteLine("{0,-20} {1,2} {2,2} {3,2} {4,4} {5,3} {6,5:#0.00} {7,3} {8,2} {9,2}", 
                                  player.KnownAs, rec.Matches, rec.Innings, rec.NotOut, rec.Runs, 
                                  rec.Highest, rec.Average, rec.Centuries, rec.Fifties, rec.Ducks);
            }
            


        }

    }
}
