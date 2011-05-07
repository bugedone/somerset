using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spider.Domain
{
    public interface IMatchDetailsRepository
    {
        void Add(ScorecardDetails details);
        void Update(ScorecardDetails details);
        void Remove(ScorecardDetails details);
        ScorecardDetails GetById(int id);
        ScorecardDetails GetByMatchCode(string matchCode);
        ICollection<ScorecardDetails> GetBySeason(string season);
    }
}
