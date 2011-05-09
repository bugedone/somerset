using System;
using Spider.Persistence;

namespace Spider.Commands
{
    class HelpCommand : ICommand
    {
        public void Execute(FileStore dataStore)
        {
            PrintHelp();
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("    crawl [start] [end]            Crawl the CricketArchive site. Optionally provide start and end seasons.");
            Console.WriteLine("    recheck [start] [end]          Revisit classification pages for exisiting records and update with changes.");
            Console.WriteLine("    download [start] [end]         Download scorecards");
            Console.WriteLine("    parse [start] [end]            Parse downloaded scorecards");
            Console.WriteLine("    parsematch <match-code>        Parse one downloaded scorecard");
            Console.WriteLine("    generate <stat> [start] [end]  Generate the statistics tables from parsed scorecards");
            Console.WriteLine("    seasons                        Reload seasons list");
            Console.WriteLine("    help                           Print this message");
            Console.WriteLine("    exit                           Exit the program");
            Console.WriteLine();
        }

    }
}
