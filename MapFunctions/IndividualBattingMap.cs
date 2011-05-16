using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spider.Domain;

namespace Spider.MapFunctions
{
    class IndividualBattingMap : IMapFunction
    {


        public IndividualBattingMap(string season)
        {
            Id = string.Format("map/batting/{0}", season);
        }

        public string Id { get; private set; }
        public void Run(CricketMatch match)
        {
            throw new NotImplementedException();
        }

        public void SaveData()
        {
            throw new NotImplementedException();
        }
    }
}
