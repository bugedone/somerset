using System;
using Raven.Client;

namespace Spider.Commands
{
    class ErrorCommand : ICommand
    {
        public string Message { get; set; }

        public void Execute(IDocumentStore dataStore)
        {
            Console.WriteLine("Error: '{0}'", Message);
        }
    }
}
