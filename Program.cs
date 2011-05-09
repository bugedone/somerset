using System;
using System.IO;
using log4net.Config;
using Spider.Commands;
using Spider.Persistence;

namespace Spider
{
    class Program
    {
        static void Main()
        {

            XmlConfigurator.Configure();

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"Cricket\");
            var store = new FileStore(path);

            bool exit = false;


            while (!exit)
            {
                Console.Write("Scorecards> ");
                string function = Console.ReadLine();
                
                switch (function)
                {
                    case "exit":
                    case "x":
                        exit = true;
                        break;
                    case "":
                        break;
                    default:
                        CommandFactory.GetCommand(function).Execute(store);
                        break;
                }

                Console.WriteLine();
            }
        }

    }
}
