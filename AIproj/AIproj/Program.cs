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
        static Dictionary<string, int> TotalCountPart = new Dictionary<string, int>();
        static Dictionary<int , DictionaryHolder> TotalPartCount = new Dictionary<int, DictionaryHolder>();
        //static bool show = false;
        static string Data = "News_Category_Dataset_v2.json";
        static int lineCount = File.ReadLines(Data).Count(); //Apskaiciuoja eiluciu kieki
        static List<Lexem> allLearning = new List<Lexem>();
        static List<Lexem> allTesting = new List<Lexem>();
        static int SegmentCount=0;

        static void Main(string[] args)
        {
            
            Console.WriteLine();
            CrossValidation();


            //            List<DataObject> all = ReadAllData(Data);
            //            List<Lexem> allLexems = GetAllLexems(all);
            //            List<Lexem> conc = GetConcentatedLexems(allLexems);
            //            CountProbabilitiesForLexems(conc);
            //;
            //            Console.WriteLine(CountProbabilityForHeadline("How 'RuPaul\u2019s Drag Race' Is Teaching Straight People About Queer Culture".ToLower(), "CRIME", conc, 3).ToString());
            //            
            Console.ReadKey();
            
        }

        public static void CrossValidation()
        {
            List<DataObject> allData = ReadAllData(Data);
            List<Lexem> allLexems = GetAllLexems(allData);
            for (int i = 0; i < 10; i++)
            {
                allLearning.Clear();
                allTesting.Clear();
                Learning(i, allLexems);
                Testing(i, allLexems);

                CountTotalCountPart(allLearning, i);
                List<Lexem> conc = GetConcentatedLexems(allLearning);
                CountProbabilitiesForPartLexems(conc, i);

                //List<Lexem> allLexemsTesting = GetAllLexems(allTesting);
                //Console.WriteLine(allLexemsTesting);
                CompareLexems(allLearning, allTesting, i);
                //Ko truksta, tai paduoti testinius duomenis.
                //Console.WriteLine(CountProbabilityForHeadline("How 'RuPaul\u2019s Drag Race' Is Teaching Straight People About Queer Culture".ToLower(), "CRIME", conc, 3).ToString());
            }
        }

        public static void Learning(int block, List<Lexem> all)
        {

            
            for (int i = 0; i < all.Count; i++)
                if (i < all.Count / 10 * block || i >= all.Count / 10 * (block + 1))
                   // using (StreamReader sr = new StreamReader(Data))
                   // {
                        for (int k = 0; k < all.Count; k++)
                        {
                            if (k == i)
                            {
                                allLearning.Add(all[i]);
                            }
                        }

                  //  }
        }

        public static void Testing(int block, List<Lexem> all)
        {
            for (int i = 0; i < all.Count; i++)
                if (i < all.Count / 10 * block || i >= all.Count / 10 * (block + 1))

                        for (int k = 0; k < all.Count; k++)
                        {
                            if (k == i)
                            {
                                allTesting.Add(all[i]);

                            }
                        }
        }


        public static void CountTotalCountPart(List<Lexem> all, int part)
        {
            TotalPartCount[part] = new DictionaryHolder();
            foreach (Lexem lex in all)
            {
                string cat = lex.counts.Keys.ElementAt(0);
                int val = 0;
                TotalPartCount[part].TotalCount.TryGetValue(cat, out val);
                TotalPartCount[part].TotalCount[cat] = val + 1;
            }
        }



        static double CountProbabilityForHeadline(string headline, string topic, List<Lexem> all, int N)
        {
            List<string> words = new List<string>();
            Regex rgx = new Regex("[a-zA-Z0-9\']+");
            foreach (Match match in rgx.Matches(headline))
            {
                words.Add(match.Value);
            }
            List<double> wordProbs = new List<double>();
            foreach (string word in words)
            {
                Lexem lex = all.Find(x => x.word.Equals(word));
                if (lex == null)
                {
                    wordProbs.Add(0.4);
                }
                else
                {
                    if (!lex.vals.Keys.Contains(topic))
                        wordProbs.Add(0.4);
                    else
                        wordProbs.Add(lex.vals[topic]);
                }

            }
            wordProbs.Sort();
            List<double> actualVals = new List<double>();
            for (int i = wordProbs.Count-1; i >= 0 && actualVals.Count != N; i--)
            {
                actualVals.Add(wordProbs[i]);
            }

            double sum1 = 1;
            double sum2 = 1; 
            foreach (double prob in actualVals)
            {
                sum1 *= prob;
                sum2 *= (1 - prob);
            }
            return sum1 / (sum1 + sum2);

        }

        static void CompareLexems(List<Lexem> lexPart, List<Lexem> remain, int part)
        {
            int correct = 0;
            int all = remain.Count;
            foreach (Lexem lex in remain)
            {
                string cat = lex.counts.Keys.ElementAt(0);
                Lexem actual = lexPart.Find(x => x.word == lex.word);
                if (actual != null && actual.vals.Keys.Contains(cat))
                {
                    double prob = actual.vals[cat];
                    if (prob >= 0.5)
                    {
                        correct += 1;
                    }
                }
            }
            SegmentCount += 1;
            Console.WriteLine("-----------------------------");
            Console.WriteLine("      "+SegmentCount+" SEGMENTAS");
            Console.WriteLine("-----------------------------");
            Console.WriteLine("  BENDRAS TIKSLUMAS " + String.Format("{0:0.00}", ((double)correct / all) * 100)+ "%");
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
                    headlineLexems.Add(match.Value.ToLower());
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

        static void CountProbabilitiesForPartLexems(List<Lexem> allPart, int part)
        {
            foreach (Lexem lex in allPart)
            {
                foreach (string key in lex.counts.Keys)
                {
                    double pwkey = (double)lex.counts[key] / TotalPartCount[part].TotalCount[key];
                    double sumpw = pwkey;

                    foreach (string otherKey in lex.counts.Keys)
                    {
                        if (!otherKey.Equals(key))
                        {
                            double pw = (double)lex.counts[otherKey] / TotalPartCount[part].TotalCount[otherKey];
                            sumpw += pw;
                        }
                    }
                    lex.vals[key] = (double)pwkey / sumpw;
                }
            }
        }

        static void CountProbabilitiesForLexems(List<Lexem> all)
        {
            foreach (Lexem lex in all)
            {
                foreach (string key in lex.counts.Keys)
                {
                    double pwkey = (double)lex.counts[key] / TotalCount[key];
                    double sumpw = pwkey;

                    foreach (string otherKey in lex.counts.Keys)
                    {
                        if (!otherKey.Equals(key))
                        {
                            double pw = (double)lex.counts[otherKey] / TotalCount[otherKey];
                            sumpw += pw;
                        }
                    }
                    lex.vals.Add(key, (double)pwkey / sumpw);
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
