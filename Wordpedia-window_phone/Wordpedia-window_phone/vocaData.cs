using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordpedia_window_phone
{
    class vocaData
    {
        public String Title { get; set; }
        public String Date { get; set; }
        public String Translate { get; set; }

        List<wordData> list = new List<wordData>();


        public List<wordData> List
        {
            get { return list; }
            set { list = value; }
        }
    }
}
