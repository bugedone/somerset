using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spider.Domain
{
    class BattingRecord
    {
        public string PlayerId { get; set; }
        public string Team { get; set; }

        public int Runs { get; set; }
        public int Minutes { get; set; }
        public int Balls { get; set; }
        public int Fours { get; set; }
        public int Sixes { get; set; }

        public int Matches { get; set; }
        public int Innings { get; set; }
        public int NotOut { get; set; }

        public float Average { get; set; }
        public int Centuries { get; set; }
        public int Fifties { get; set; }
        public int Highest { get; set; }
        public int Ducks { get; set; }

        public int Bowled { get; set; }
        public int Caught { get; set; }
        public int Lbw { get; set; }
        public int RunOut { get; set; }
        public int HitWicket { get; set; }
        public int Stumped { get; set; }
        public int Other { get; set; }
        public int Retired { get; set; }
        public int Absent { get; set; }

    }
}
