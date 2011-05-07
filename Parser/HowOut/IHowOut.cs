using System.Collections.Generic;
using Spider.Domain;

namespace Spider.Parser.HowOut
{
    public interface IHowOut
    {
        string ScorecardMatch { get; }
        string DisplayName { get; }
        HowOutType HowOutType { get; }
        bool IsOut { get; }
        bool IsInnings { get; }
        bool HasFielder { get; }
        bool HasBowler { get; }
        string RenderPattern { get; }
        string GetFielder(IList<string> players);
        string GetBowler(IList<string> players);
    }
}
