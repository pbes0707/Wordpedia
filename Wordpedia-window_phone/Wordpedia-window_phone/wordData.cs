using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordpedia_window_phone
{
    class wordData
    {
        public wordData(string _word)
        {
            this._word = _word;
            this._count = 1;
        }
        private string _word;
        private int _count;

        public string Word
        {
            get { return _word; }
            set { _word = value; }
        }
        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }
    }
}
