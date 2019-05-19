using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIproj
{
    class DataObject
    {
        public string category { get; private set; }
        public string headline { get; private set; }
        public string authors { get; private set; }
        public string link { get; private set; }
        public string short_description { get; private set; }
        public string date { get; private set; }
        public DataObject(string category, string headline, string authors, string link, string short_description, string date)
        {
            this.category = category;
            this.headline = headline;
            this.authors = authors;
            this.link = link;
            this.short_description = short_description;
            this.date = date;
        }
    }
}
