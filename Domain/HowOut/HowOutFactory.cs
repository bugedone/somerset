using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;

namespace Spider.Domain.HowOut
{
    class HowOutFactory
    {
        private static readonly ILog _log = LogManager.GetLogger("Spider");
        private static readonly Dictionary<string, IHowOut> _registry;

        static HowOutFactory()
        {
            _registry = new Dictionary<string, IHowOut>();

            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                Type iface = t.GetInterface("Spider.Domain.HowOut.IHowOut");
                if (iface != null)
                {
                    IHowOut ho = (IHowOut)Activator.CreateInstance(t);
                    _registry.Add(ho.ScorecardMatch, ho);
                }
            }
        }

        public static IHowOut GetHowOut(string scorecardMatch)
        {
            if (!_registry.ContainsKey(scorecardMatch))
            {
                _log.WarnFormat("Could not find how out type '{0}'", scorecardMatch);
                return Unknown;
            }
            return _registry[scorecardMatch];
        }


        public static IHowOut Unknown { get { return new Unknown(); } }
        public static IHowOut DNB { get { return new DidNotBat(); } }
    }
}
