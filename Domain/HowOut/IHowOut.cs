using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spider.Domain.HowOut
{
    public interface IHowOut
    {
        string ScorecardMatch { get; }
        string DisplayName { get; }
        bool IsOut { get; }
        bool IsInnings { get; }
        bool HasFielder { get; }
        bool HasBowler { get; }
        string RenderPattern { get; }
        PlayerDetails GetFielder(IList<PlayerDetails> players);
        PlayerDetails GetBowler(IList<PlayerDetails> players);
    }
}
