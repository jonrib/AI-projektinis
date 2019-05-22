using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIproj
{
    class DictionaryHolder
    {
        public Dictionary<string, int> TotalCount { get; set; }
        public DictionaryHolder()
        {
            TotalCount = new Dictionary<string, int>();
        }
    }
}
