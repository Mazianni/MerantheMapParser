using System.Text.RegularExpressions;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace MerantheMapParser
{

    public static class EnumerableEx
    {
        public static IEnumerable<string> SplitBy(this string str, int chunkLength)
        {
            if (String.IsNullOrEmpty(str)) throw new ArgumentException();
            if (chunkLength < 1) throw new ArgumentException();

            for (int i = 0; i < str.Length; i += chunkLength)
            {
                if (chunkLength + i > str.Length)
                    chunkLength = str.Length - i;

                yield return str.Substring(i, chunkLength);
            }
        }
    }
    public class Parser
    {
        static string MAP_PATH = "01_Meranthe.dmm";
        static List<string> MAP_LIST = new() 
        {
            "\\/ResourceNode\\/Alchemy\\/FourLeaf",
            "\\/ResourceNode\\/Alchemy\\/RopeRoot",
            "\\/ResourceNode\\/Alchemy\\/Luminite",
            "\\/ResourceNode\\/Alchemy\\/NightShadeRoot",
            "\\/ResourceNode\\/Alchemy\\/CrimsonReed",
            "\\/ResourceNode\\/Alchemy\\/SootheReed",
            "\\/ResourceNode\\/Alchemy\\/ColchicHazel",
            "\\/ResourceNode\\/Alchemy\\/Poppylus",
            "\\/ResourceNode\\/Alchemy\\/Leek",
            "\\/ResourceNode\\/Alchemy\\/Cylion",
            "\\/ResourceNode\\/Alchemy\\/AlgeronLeaf",
            "\\/ResourceNode\\/Alchemy\\/SugarCap",
            "\\/ResourceNode\\/Alchemy\\/GlowingMushroom",
            "\\/ResourceNode\\/Alchemy\\/Alessa",
            "\\/ResourceNode\\/Alchemy\\/JuniperSap",
            "\\/ResourceNode\\/Alchemy\\/Sludge",
            "\\/ResourceNode\\/Alchemy\\/Sinka",
            "\\/ResourceNode\\/Alchemy\\/CostusRoot",
            "\\/ResourceNode\\/Alchemy\\/Firefly",
            "\\/ResourceNode\\/Alchemy\\/Acorn",
            "\\/ResourceNode\\/Alchemy\\/Tumeric_Root",
            "\\/ResourceNode\\/Alchemy\\/Lavender",
            "\\/ResourceNode\\/Alchemy\\/RabbitFoot",
            "\\/ResourceNode\\/Alchemy\\/Feather",
            "\\/ResourceNode\\/Alchemy\\/Amber",
            "\\/ResourceNode\\/Alchemy\\/Scale",
            "\\/ResourceNode\\/Alchemy\\/Lotus",
            "\\/ResourceNode\\/Alchemy\\/Yokai_Horn",
            "\\/ResourceNode\\/Alchemy\\/Fairy_Dust",
            "\\/ResourceNode\\/Alchemy\\/Briarwood",
            "\\/ResourceNode\\/Alchemy\\/Herdaia",
            "\\/ResourceNode\\/Alchemy\\/Cobalthine",
            "\\/ResourceNode\\/Alchemy\\/Kyiah",
            "\\/ResourceNode\\/Alchemy\\/Blanchebloom",
            "\\/ResourceNode\\/Alchemy\\/Azilaena",
            "\\/ResourceNode\\/Alchemy\\/Saniskriti",
            "\\/ResourceNode\\/Alchemy\\/Marignolia",
            "\\/ResourceNode\\/Alchemy\\/Perdegrine",
            "\\/ResourceNode\\/Alchemy\\/Aetherblossom"
        };
        Dictionary<string, List<string>> nodes = new();
        Dictionary<string, List<string>> finalnodes = new();
        static string initial_regex = "((.*)(ResourceNode\\/Alchemy\\/.*)(.))+";
        static string match_regex_firsthalf = "(.*)(";
        static string match_regex_secondhalf = ")(.)+/g";
        static string trimmer_regex = "((.*)\\/ResourceNode(.*)\\/Alchemy(.*)\\/([A-z]*))+";
        static string key_regex = "(\"([A-z]{3})\")";
        static string index_regex = "(\\([0-9],[0-9],[0-9]\\))";
        List<string> processed = new List<string>();
        string line = "";
        string indexstring = "";

        static IEnumerable<string> WholeChunks(string str, int chunkSize)
        {
            for (int i = 0; i < str.Length; i += chunkSize)
                yield return str.Substring(i, chunkSize);
        }

        public static async Task Main()
        {
            Parser parser = new Parser();
            await parser.ParseMap();
        }
        public async Task ParseMap()
        {

            StreamReader reader = File.OpenText(MAP_PATH);

            string NL = Environment.NewLine; // shortcut
            string NORMAL = Console.IsOutputRedirected ? "" : "\x1b[39m";
            string RED = Console.IsOutputRedirected ? "" : "\x1b[91m";
            string GREEN = Console.IsOutputRedirected ? "" : "\x1b[92m";
            string YELLOW = Console.IsOutputRedirected ? "" : "\x1b[93m";
            string BLUE = Console.IsOutputRedirected ? "" : "\x1b[94m";
            string MAGENTA = Console.IsOutputRedirected ? "" : "\x1b[95m";
            string CYAN = Console.IsOutputRedirected ? "" : "\x1b[96m";
            string GREY = Console.IsOutputRedirected ? "" : "\x1b[97m";
            string BOLD = Console.IsOutputRedirected ? "" : "\x1b[1m";
            string NOBOLD = Console.IsOutputRedirected ? "" : "\x1b[22m";
            string UNDERLINE = Console.IsOutputRedirected ? "" : "\x1b[4m";
            string NOUNDERLINE = Console.IsOutputRedirected ? "" : "\x1b[24m";
            string REVERSE = Console.IsOutputRedirected ? "" : "\x1b[7m";
            string NOREVERSE = Console.IsOutputRedirected ? "" : "\x1b[27m";

            Console.WriteLine($"{MAGENTA}" + DateTime.Now.ToString() + $": Populating results dictionary...{NORMAL}");

            foreach (string s in MAP_LIST) //Populate results dictionary.
            {
                if (!nodes.ContainsKey(s))
                {
                    nodes.Add(s, new List<string>());
                    Regex rx = new Regex(trimmer_regex);
                    Match match = rx.Match(s);
                    if (match.Success)
                    {
                        finalnodes.Add(match.Groups[5].ToString(), new List<string>());
                    }

                }
                if (s == MAP_LIST[MAP_LIST.Count - 1])
                {
                    Console.WriteLine($"{MAGENTA}" + DateTime.Now.ToString() + $": Results dictionary populated.{NORMAL}");
                }
            }

            Console.WriteLine($"{MAGENTA}" + DateTime.Now.ToString() + $": Doing initial map parse...{NORMAL}");
            line = await reader.ReadToEndAsync();
            Regex reg = new Regex(initial_regex, RegexOptions.Compiled);
            MatchCollection matcher = reg.Matches(line);
            foreach (Match match in matcher)
            {
                if (match.Success)
                {
                    processed.Add(match.Value);
                }
            }
            Regex index = new Regex(index_regex, RegexOptions.Multiline | RegexOptions.Compiled);
            Match indexmatch = index.Match(line);
            if (indexmatch.Success)
            {
                indexstring = line.Substring(indexmatch.Index);
            }
            indexstring = indexstring.Remove(0, 14);
            indexstring = indexstring.Remove(indexstring.Length - 4, 4);
            indexstring = indexstring.Replace("\n", "");
            indexstring = indexstring.Replace("\r", "");
            Console.WriteLine($"{MAGENTA}" + DateTime.Now.ToString() + $": Done doing initial map parse.{NORMAL}");

            Console.WriteLine($"{MAGENTA}" + DateTime.Now.ToString() + $": Sorting keys into lists...{NORMAL}");

            foreach (string processed_string in processed) //Sort each string with a resource node into sub lists.
            {
                foreach (string s in MAP_LIST)
                {
                    Regex rx = new Regex(match_regex_firsthalf + s + match_regex_secondhalf, RegexOptions.Compiled);
                    Match matching = rx.Match(processed_string);
                    List<string> add_list = nodes[s];
                    if (matching.Success)
                    {
                        Regex rx2 = new Regex(key_regex, RegexOptions.Compiled);
                        Match matching2 = rx2.Match(processed_string);
                        if (matching.Success)
                        {
                            add_list.Add(matching2.Groups[2].ToString());
                        }
                    }
                }
            }
            Console.WriteLine($"{MAGENTA}" + DateTime.Now.ToString() + $": Done sorting keys into lists.{NORMAL}");

            Console.WriteLine($"{MAGENTA}" + DateTime.Now.ToString() + $": Indexing map data...{NORMAL}");
            int chunkindex = 1;
            string[,] map = new string[1001, 1001];
            foreach (var chunk in WholeChunks(indexstring, 3))
            {
                var x = chunkindex / 1000;
                var y = chunkindex % 1000;
                map[x, y] = chunk;
                chunkindex++;
            }

            Console.WriteLine($"{MAGENTA}" + DateTime.Now.ToString() + $": Done indexing map data.{NORMAL}");

            Console.WriteLine($"{MAGENTA}" + DateTime.Now.ToString() + $": Generating XY pairs from keys and map data...{NORMAL}");
            for (int i = 1;i <= 1000001; i++) 
            {
                var x = i / 1000;
                var y = i % 1000;
                string chunk = map[x,y];
                foreach (string s in nodes.Keys)
                {
                    if (nodes[s].Count == 0) { continue; }
                    foreach (string l in nodes[s])
                    {
                        if(chunk == l)
                        {
                            Console.WriteLine(l);
                            Console.WriteLine(chunk);
                            Console.WriteLine("Resource " + s + $" Key {l}" + " X:" + x.ToString() + " Y: " + y.ToString());
                        }
                    }
                    break;
                }
            }
        }

    }
}
