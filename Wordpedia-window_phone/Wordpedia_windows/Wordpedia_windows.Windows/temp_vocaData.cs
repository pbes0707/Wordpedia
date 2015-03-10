using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Wordpedia_windows
{
    class temp_vocaData
    {
        public int Kind { get; set; } // kind : 1 이면 ImageCapture 2 이면 File
        public String Title { get; set; }
        public DateTime Date { get; set; }
        public String originalWords { get; set; }
        public String translatedWords { get; set; }
        public List<temp_wordData> Words { get; set; }

        public String Path { get; set; }
        public String Article { get; set; }

    }

    public sealed class SQLvocaData 
    {
        /// <summary>
        /// You can create an integer primary key and let the SQLite control it.
        /// </summary>
        /// 
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int Kind { get; set; } // kind : 1 이면 ImageCapture 2 이면 File
        public String Title { get; set; }
        public DateTime Date { get; set; }
        public String[] originalWords { get; set; }
        public String translatedWords { get; set; }
        public String JsonWords { get; set; }

        public byte Path { get; set; }
        public String Article { get; set; }
    }
}
