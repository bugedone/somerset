using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spider.Domain
{
    public struct OverType
    {
        public int Overs { get; set; }
        public int Balls { get; set; }
        public int BallsPerOver { get; set; }

        public int TotalBalls 
        {
            get
            {
                return (Overs * BallsPerOver) + Balls;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}", Overs, Balls);
        }
    }
}
