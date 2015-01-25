using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordpedia_window_phone
{
    class wordData
    {
        public wordData(String _word, String _translateWord)
        {
            Word = _word;
            TranslateWord = _translateWord;
            Count = 1;
        }
        public String Word {get; set;}
        public String TranslateWord { get; set; }
        public int Count {get; set;}
    }
}
