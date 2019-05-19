using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIproj
{
    class Lexem : IEquatable<Lexem>, IComparable<Lexem>
    {
        public string word { get; set; }
        public Dictionary<string, double> vals { get; set; }
        public Dictionary<string, int> counts { get; set; }
        public Lexem(string word)
        {
            this.word = word;
            vals = new Dictionary<string, double>();
            counts = new Dictionary<string, int>();
        }

        public bool Equals(Lexem other)
        {
            return this.word.Equals(other.word);
        }

        public int CompareTo(Lexem other)
        {
            return this.word.CompareTo(other.word);
        }
    }
}
