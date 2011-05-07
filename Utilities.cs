using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spider
{
    static class Utilities
    {
        public static string ToOrdinal(this int cardinal)
        {
            string number = cardinal.ToString();
            string ending = "th";

            if (number.EndsWith("1") && !number.EndsWith("11"))
                ending = "st";
            if (number.EndsWith("2") && !number.EndsWith("12"))
                ending = "nd";
            if (number.EndsWith("3") && !number.EndsWith("13"))
                ending = "rd";

            return number + ending;
        }

    }
}
