using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Spider.Commands
{
    static class CommandFactory
    {
        public static ICommand GetCommand(string commandLine)
        {
            string[] parts = Regex.Split(commandLine, @"\s+");

            if (parts.Length == 0)
                return new ErrorCommand { Message = "No command provided."};

            string function = parts[0];
            string startSeason = (parts.Length < 2 ? DateTime.Now.Year.ToString() : parts[1]);

            switch (function)
            {
                case "help":
                    return new HelpCommand();
                case "crawl":
                    return new CrawlCommand
                                {
                                    StartSeason = startSeason,
                                    EndSeason = (parts.Length < 3 ? startSeason : parts[2])
                                };
                case "recheck":
                    return new RecheckCommand
                                {
                                    StartSeason = startSeason,
                                    EndSeason = (parts.Length < 3 ? startSeason : parts[2])
                                };
                case "download":
                    return new DownloadCommand
                                {
                                    StartSeason = startSeason,
                                    EndSeason = (parts.Length < 3 ? startSeason : parts[2])
                                };
                case "parse":
                    return new ParseCommand
                    {
                        StartSeason = startSeason,
                        EndSeason = (parts.Length < 3 ? startSeason : parts[2])
                    };
                case "parsematch":
                    if (parts.Length < 2)
                        return new ErrorCommand { Message = "No match code provided" };

                    return new ParseMatchCommand { MatchCode = parts[1] };
                case "generate":
                    return new GenerateStatisticsCommand
                                {
                                    StartSeason = startSeason,
                                    EndSeason = (parts.Length < 3 ? startSeason : parts[2])
                                };
                case "seasons":
                    return new GetSeasonsCommand();
                default:
                    return new ErrorCommand { Message = string.Format("Unknown command '{0}'", function) };
            }
        }
    }
}
