using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spider.Domain
{
    class BowlingRecord
    {
        public string PlayerId { get; set; }
        public string Team { get; set; }

        public int BallsBowled { get; set; }
        public int Maidens { get; set; }
        public int Runs { get; set; }
        public int Wickets { get; set; }
        public int Wides { get; set; }
        public int NoBalls { get; set; }

    }
}
