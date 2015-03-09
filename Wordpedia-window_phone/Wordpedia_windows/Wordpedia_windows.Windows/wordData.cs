using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordpedia_windows
{
    class wordData
    {
        public wordData(String _word, String _translateWord)
        {
            OriginalWord = _word;
            TranslateWord = _translateWord;
            Count = 1;
        }
        public String OriginalWord {get; set;}
        public String TranslateWord { get; set; }
        public int Count {get; set;}
    }
}
