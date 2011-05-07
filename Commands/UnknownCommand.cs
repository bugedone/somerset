using System;
using Spider.Persistence;

namespace Spider.Commands
{
    class ErrorCommand : ICommand
    {
        public string Message { get; set; }

        public void Execute(FileStore dataStore)
        {
            Console.WriteLine("Error: '{0}'", Message);
        }
    }
}
