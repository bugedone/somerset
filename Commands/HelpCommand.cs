using System;
using Raven.Client;

namespace Spider.Commands
{
    class HelpCommand : ICommand
    {
        public void Execute(IDocumentStore dataStore)
        {
            PrintHelp();
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("    crawl      Crawl the CricketArchive site. Optionally provide start and end seasons.");
            Console.WriteLine("    recheck    Revisit classification pages for exisiting records and update with changes.");
            Console.WriteLine("    download   Download scorecards");
            Console.WriteLine("    parse      Parse downloaded scorecards");
            Console.WriteLine("    parsematch Parse one downloaded scorecard");
            Console.WriteLine("    generate   Generate the statistics tables from parsed scorecards");
            Console.WriteLine("    seasons    Reload seasons list");
            Console.WriteLine("    help       Print this message");
            Console.WriteLine("    exit       Exit the program");
            Console.WriteLine();
        }

    }
}
