using System.Collections.Generic;
using System.Linq;
using Spider.Domain;
using HtmlAgilityPack;
using log4net;
using System.Text.RegularExpressions;

namespace Spider.Parser
{
    class PlayerPageParser
    {
        private static readonly ILog Log = LogManager.GetLogger("Spider");

        public static PlayerDetails Parse(string url)
        {
            Dictionary<string, string> tokens = Tokenize(url);
            if (tokens.Count > 0)
            {
                PlayerDetails player = new PlayerDetails();

                foreach (string key in tokens.Keys)
                {
                    string value = tokens[key];
                    switch (key)
                    {
                        case "Full name:":
                            player.FullName = value;
                            break;
                        case "Known As":
                            player.KnownAs = value;
                            break;
                        case "Born:":
                            player.Born = value;
                            break;
                        case "Died:":
                            player.Died = value;
                            break;
                        case "Batting:":
                            player.Batting = value;
                            break;
                        case "Bowling:":
                            player.Bowling = value;
                            break;
                        case "Occasional wicket-keeper":
                        case "Wicket-keeper":
                            player.WicketKeeper = true;
                            break;
                        case "Somerset cap:":
                            player.SomersetCap = int.Parse(value);
                            break;
                        case "Somerset benefit season:":
                            if (value.EndsWith(" (Testimonial)"))
                                value = value.Substring(0, value.Length - 14).Trim();
                            player.Benefit = int.Parse(value);
                            break;
                        case "Relations:":
                        case "Teams:":
                        case "Biography:":
                        case "Christened:":
                        case "Buried:":
                        case "Articles:":
                        case "Pictures:":
                        case "Education:":
                        case "Other Sports:":
                        case "External link:":
                        case "Clubs:":
                        case "Disability:":
                        case "Other Relations:":
                        case "Galleries:":
                        case "Audio-Video:":
                            break;
                        default:
                            if (Regex.IsMatch(value, @"^\d{4}(?:/\d{2})?$"))
                            {
                                PlayerAward award = new PlayerAward
                                                        {
                                                            Award = key,
                                                            Season = value
                                                        };
                                if (player.Awards == null)
                                    player.Awards = new List<PlayerAward>();
                                player.Awards.Add(award);
                            }
                            else if (value.Contains(','))
                            {
                                // Could be a multi-year award
                                string[] seasons = Regex.Split(value, @"\s*,\s*");
                                foreach (string season in seasons)
                                {
                                    if (!Regex.IsMatch(value, @"\d{4}(?:/\d{2})?"))
                                    {
                                        Log.WarnFormat("Player {0} has previously unseen token '{1}'", player.FullName, key);
                                        break;
                                    }

                                    PlayerAward award = new PlayerAward
                                    {
                                        Award = key,
                                        Season = season
                                    };
                                    if (player.Awards == null)
                                        player.Awards = new List<PlayerAward>();
                                    player.Awards.Add(award);
                                }
                            }
                            else
                            {
                                Log.WarnFormat("Player {0} has previously unseen token '{1}'", player.FullName, key);
                            }
                            break;
                        }
                }

                return player;
            }

            return null;
        }

        private static Dictionary<string, string> Tokenize(string url)
        {
            Dictionary<string, string> tokens = new Dictionary<string, string>();
            HtmlNode contentDiv = WebClient.GetWebContentNode(url);
            if (contentDiv == null)
                return tokens;

            HtmlNode titleNode = contentDiv.SelectSingleNode(".//h2");
            HtmlNodeCollection tableRows;
            if (titleNode != null)
            {
                tokens.Add("Known As", GetTrimmedInnerText(titleNode));
                tableRows = titleNode.SelectNodes("../table[1]//tr");
            }
            else
                tableRows = contentDiv.SelectNodes("./table[1]//tr");

            foreach (HtmlNode row in tableRows)
            {
                HtmlNodeCollection cells = row.SelectNodes("./td");
                string name = GetTrimmedInnerText(cells[0]);
                if (cells.Count == 2 && !string.IsNullOrEmpty(name))
                {
                    string value = GetTrimmedInnerText(cells[1]);
                    if (tokens.ContainsKey(name))
                        tokens[name] += ", " + value;
                    else
                        tokens.Add(name, value);
                }
                else if (cells.Count == 1)
                {
                    string content = cells[0].InnerText;
                    if (content.Contains(":"))
                    {
                        if (content.StartsWith("Somerset cap:"))
                            tokens.Add("Somerset cap:", content.Substring(14).Trim());
                        else if (content.StartsWith("Somerset benefit season:"))
                            tokens.Add("Somerset benefit season:", content.Substring(25));
                        else if (cells[0].SelectSingleNode("./a") != null)
                        {
                            string key = content.Substring(0, content.IndexOf(":")).Trim();
                            string value = content.Substring(content.IndexOf(":") + 1).Trim();
                            tokens.Add(key, value);
                        }
                    }
                }
            }

            return tokens;
        }


        private static string GetTrimmedInnerText(HtmlNode node)
        {
            string text = node.InnerText;
            text = text.Replace("&nbsp;", " ");
            text = text.Trim();
            return text;
        }

    }
}
