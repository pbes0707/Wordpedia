using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordpedia_windows
{
    class vocaData
    {
        public String creator {get; set;}
        public String title { get; set; }
        public int refs { get; set; } //// 모름 참조된 개수인듯
        public int id { get; set; } //// 단어장의 고유 아이디
        public List<String> from { get; set; }
        public String to { get; set; }
        public String creatorToken { get; set; }
        public String createDate { get; set; }

        public List<String> words { get; set; }
        public List<String> translatedWords { get; set; }

    }

    class userData
    {
        public String id { get; set; } //
        public String token { get; set; }

        public List<vocaData> collections { get; set; }
    }
}
