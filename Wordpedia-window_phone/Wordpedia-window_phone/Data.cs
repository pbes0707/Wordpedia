using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordpedia_window_phone
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
        public List<Comment> comments { get; set; }


        String _fullTranslate;

        public String fullTranslate
        {
            get {
                String s;
                s = from[0];
                for(int i = 1 ; i <from.Count ; i++)
                    s += "," + from[i];
                s += "-" + to;
                return s;
            }
            set { _fullTranslate = value; }
        }


        List<Word> _wordList;

        public List<Word> wordList
        {
            get
            {
                List<Word> temp = new List<Word>();
                for (int i = 0; i < words.Count; i++ )
                {
                    Word v = new Word();
                    v.word = words[i];
                    v.translateWord = translatedWords[i];
                    temp.Add(v);
                }
                return temp;
            }
            set { _wordList = value; }
        }
    }

    class userData
    {
        public String id { get; set; } //
        public String token { get; set; }
        public String userId { get; set; }

        public List<vocaData> collections { get; set; }
    }
    class Comment
    {
        public String createDate { get; set; }
        public String comment { get; set; }
        public String creator { get; set; }
    }
    class Word
    {
        public String word {get;set;}
        public String translateWord {get; set;}
    }

    class LanguageCode
    {
        public static string[] getLanguageCode()
        {
            String[] separators = { "," };
            return "ar,bg,ca,zh-CHS,zh-CHT,cs,da,nl,en,et,fi,fr,de,el,ht,he,hi,mww,hu,id,it,ja,tlh,tlh-Qaak,ko,lv,lt,ms,mt,no,fa,pl,pt,ro,ru,sk,sl,es,sv,th,tr,uk,ur,vi,cy".Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }
        public static string[] getLanguageName()
        {
            String[] separators = { "," };
            return "Arabic,Bulgarian,Catalan,Chinese Simplified,Chinese Tranditional,Czech,Danish,Dutch,English,Estonian,Finnish,French,German,Greek,Haitian Creole,Hebrew,Hindi,Hmong Daw,Hungarian,Indonesian,Italian,Japanese,Klingon,Kligon (plqaD),Korean,Latvian,Lithuanian,Malay,Maltese,Norwegian,Persian,Polish,Portuguese,Romanian,Russian,Slovak,Slovenian,Spanish,Swedish,Thai,Turkish,Ukrainian,Urdu,Vietnamese,Welsh".Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }

        public String Language { get; set; }
        public int Code { get; set; }
    }
}
