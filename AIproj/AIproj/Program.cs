using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AIproj
{
    class Program
    {
        static Dictionary<string, int> TotalCount = new Dictionary<string, int>();

        static void Main(string[] args)
        {
            List<DataObject> all = ReadAllData("News_Category_Dataset_v2.json");
            List<Lexem> allLexems = GetAllLexems(all);
            List<Lexem> conc = GetConcentatedLexems(allLexems);
            CountProbabilitiesForLexems(conc);
            Console.ReadKey();
        }

        static List<Lexem> GetConcentatedLexems(List<Lexem> all)
        {
            List<Lexem> concentated = new List<Lexem>();
            all.Sort();
            for (int i = 0; i < all.Count; i++)
            {
                Lexem lex = all[i];
                while (i+1 < all.Count && all[i+1].word == lex.word)
                {
                    Lexem next = all[i + 1];
                    foreach(string key in next.counts.Keys)
                    {
                        int val = 0;
                        lex.counts.TryGetValue(key, out val);
                        lex.counts[key] = val + 1;
                    }
                    i++;
                }
                concentated.Add(lex);
            }
            return concentated;
        }

        static List<Lexem> GetAllLexems(List<DataObject> data)
        {
            List<Lexem> lexems = new List<Lexem>();
            Regex rgx = new Regex("[a-zA-Z0-9\']+");
            foreach (DataObject obj in data)
            {
                List<string> headlineLexems = new List<string>();
                foreach (Match match in rgx.Matches(obj.headline))
                {
                    headlineLexems.Add(match.Value);
                }
                foreach (string headlineW in headlineLexems)
                {
                    Lexem lex = new Lexem(headlineW);
                    lex.counts[obj.category] = 1;
                    lexems.Add(lex);

                    int val = 0;
                    TotalCount.TryGetValue(obj.category, out val);
                    TotalCount[obj.category] = val + 1;
                }

                

            }
            return lexems;
        }

        static void CountProbabilitiesForLexems(List<Lexem> all)
        {
            foreach (Lexem lex in all)
            {
                foreach (string key in lex.counts.Keys)
                {
                    double pwkey = (lex.counts[key] / TotalCount[key])*100;
                    double sumpw = pwkey; 

                    foreach (string otherKey in lex.counts.Keys)
                    {
                        if (!otherKey.Equals(key))
                        {
                            double pw = lex.counts[otherKey] / TotalCount[otherKey];
                            sumpw += pw;
                        }
                    }

                    lex.vals.Add(key, pwkey / sumpw);
                }
            }
        }

        static List<DataObject> ReadAllData(string data)
        {
            List<DataObject> all = new List<DataObject>();
            using (StreamReader reader = new StreamReader(data))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    DataObject m = JsonConvert.DeserializeObject<DataObject>(line);
                    all.Add(m);
                }
            }
            return all;
        } 
    }
}
