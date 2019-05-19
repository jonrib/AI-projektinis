using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AIproj
{
    class Program
    {
        static void Main(string[] args)
        {
            List<DataObject> all = ReadAllData("News_Category_Dataset_v2.json");
            Console.ReadKey();
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
