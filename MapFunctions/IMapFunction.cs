using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spider.Domain;

namespace Spider.MapFunctions
{
    interface IMapFunction
    {
        string Id { get; }

        void Run(CricketMatch match);

        void SaveData();
    }
}
