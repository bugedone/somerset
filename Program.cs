using System;
using System.Reflection;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework.Config;
using log4net.Config;
using Raven.Client.Document;
using Spider.Commands;

namespace Spider
{
    class Program
    {
        static void Main()
        {

            XmlConfigurator.Configure();
            //ActiveRecordStarter.Initialize(Assembly.GetExecutingAssembly(), ActiveRecordSectionHandler.Instance);

            var store = new DocumentStore { Url = "http://localhost:8080" };
            store.Initialize();

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
