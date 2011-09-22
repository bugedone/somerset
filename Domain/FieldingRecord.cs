using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spider.Domain
{
    class FieldingRecord
    {
        public string PlayerId { get; set; }
        public string Team { get; set; }

        public int Catches { get; set; }
        public int Stumpings { get; set; }
    }
}
