using System;
using Raven.Client;

namespace Spider.Commands
{
    class ParseMatchCommand : ICommand
    {
        public string MatchCode { get; set; }

        public void Execute(IDocumentStore dataStore)
        {
            throw new NotImplementedException();
        }
    }
}
