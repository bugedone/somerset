using System;
using Spider.Persistence;

namespace Spider.Commands
{
    class ParseMatchCommand : ICommand
    {
        public string MatchCode { get; set; }

        public void Execute(FileStore dataStore)
        {
            throw new NotImplementedException();
        }
    }
}
