using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WindowsPreview.Media.Ocr;
using SQLite;
using Windows.Web.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Wordpedia_window_phone
{
    public sealed partial class CreateVocabulary : Page
    {
        private SQLiteConnection conn;
        private TransmitData data;

        public CreateVocabulary()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            data = (TransmitData)e.Parameter;

            Process(data.Article);

        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (conn != null)
                conn.Close();
        }

        private void Process(String article)
        {
            ////////////////////////////String Split/////////////////////////////
            String lowerArticle = article.ToLower();
            String[] separators = { ",", ".", "!", "?", ";", ":", " ", "\r", "\n", "\t" };
            String[] words = lowerArticle.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            ///////////////////////////List Create////////////////////////////////
            List<wordData> wordList = new List<wordData>();
            foreach (string v in words)
            {
                bool _flag = false;
                if (wordList.Count != 0)
                {
                    foreach (wordData c in wordList)
                    {
                        if (c.Word == v)
                        {
                            c.Count++;
                            _flag = true;
                            break;
                        }
                    }
                }
                if (_flag == false || wordList.Count == 0)
                    wordList.Add(new wordData(v, v));
            }
            /////////////////////Word Translate Request to Server/////////////
            /*String url = "http://wordpedia.herokuapp.com/translate/g/ko?";
            foreach(wordData v in wordList)
            {
                if (wordList[0] != v)
                    url += "&";
                url += "w=" + v.Word;
            }
            Uri strUri = new Uri(url);
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                String ResponseString = await httpClient.GetStringAsync(strUri);
                Dictionary<string, > jsonArray = JsonConvert.DeserializeObject(ResponseString);
            }
            catch(Exception ex)
            {

            }*/
            //////////////////////Voca Data Create//////////////////////////
            ///////////Kind  1 : Image            2 : HyperLink/////////////
            ///////////Path  1 : Image local Path 2 : HyperLink address///// 
            vocaData vocadata = new vocaData()
            {
                Kind = data.Spec,
                Title = article.Substring(0, 15),
                Date = DateTime.Now,
                Translate = "en-kr",
                Words = wordList,

                Path = data.Path,
                Article = lowerArticle,
            };
            ////////////////// vocaData -> SQLvocaData ///////////////////

            string json = JsonConvert.SerializeObject(vocadata.Words);
            SQLvocaData sqlvocadata = new SQLvocaData()
            {
                Kind = vocadata.Kind,
                Title = vocadata.Title,
                Date = vocadata.Date,
                Translate = vocadata.Translate,
                JsonWords = json,

                Path = vocadata.Path,
                Article = vocadata.Article,
            };

            ///////////////////SQLite DB 에 저장한다.//////////////////////

            string strConn = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "Vocabulary.sqlite")); ;

            conn = new SQLiteConnection(strConn);
            conn.CreateTable<SQLvocaData>();
            List<SQLvocaData> retrievedTasks = conn.Table<SQLvocaData>().ToList<SQLvocaData>();
            conn.Insert(sqlvocadata);
            conn.Close();

            this.Frame.Navigate(typeof(Vocabulary), vocadata);
        }
    }

    public class TransmitData
    {
        public int Spec { get; set; } ///// 1 : img   2 : HyperLink
        public String Article { get; set; }
        public String Path { get; set; } ////// img : Image Local Path    HyperLink : HTML Source Code
    }
}
