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
using System.Text.RegularExpressions;
using System.Net;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Wordpedia_window_phone
{
    public sealed partial class CreateVocabulary : Page
    {
        private TransmitData data;
        private String[] words;

        public CreateVocabulary()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            data = (TransmitData)e.Parameter;

           await Process(data.Article);

        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
        }

        private async Task Process(String article)
        {
            ////////////////////////////String Split/////////////////////////////
            String transArticle = article.ToLower();
            /*Regex regex = new Regex("<[^>]*>", RegexOptions.IgnoreCase);
            String result = regex.Replace(htmlContent, "");
            result = result.Replace("\n", "");*/
            transArticle = Regex.Replace(transArticle, @"[^a-zA-Z0-9가-힣]", ",", RegexOptions.IgnoreCase);
            String[] separators = { "," };
            words = transArticle.Split(separators, StringSplitOptions.RemoveEmptyEntries);


            String title = new Random().Next(0, 10000).ToString();
            String to = "ko";
            List<String> fixed_words = new List<String>();
            foreach (String v in words)
            {
                if (v.Length < 2) continue;
                bool flag = false;
                foreach (String w in fixed_words)
                    if (v.Equals(w) == true)
                        flag = true;
                if (flag == false)
                    fixed_words.Add(v);
            }
            String query = fixed_words[0];
            for (int i = 1; i < fixed_words.Count; i++)
            {
                query += "&w=" + fixed_words[i];
            }


            if(words[0] == null)
            {
                this.Frame.Navigate(typeof(Library));
                return;
            }

            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            string url = "http://wordpedia.herokuapp.com/collection/create?title=" + title + "&to=" + to + "&w=" + query;
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            request.Headers["token"] = localSettings.Values["token"].ToString();
            try
            {
                WebResponse response = await request.GetResponseAsync();
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    String responseString = reader.ReadToEnd();
                    Dictionary<string, string> jsonArray =
                        JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);
                    String state;
                    if (jsonArray["requestCode"].Equals("1"))
                    {
                        state = "SUCCESS";

                        StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                        StorageFile file = await folder.CreateFileAsync(jsonArray["collectionId"].ToString() + ".file");
                        var streams = await file.OpenAsync(FileAccessMode.ReadWrite);

                        //////////////////단어장 하나 받아와서 Vocabulary 열기///////////////////
                        {
                            string url2 = "http://wordpedia.herokuapp.com/get/collection?collectionId="+jsonArray["collectionId"].ToString();
                            WebRequest request2 = WebRequest.Create(url2);
                            request2.Method = "POST";
                            request2.Headers["token"] = localSettings.Values["token"].ToString();
                            try
                            {
                                WebResponse response2 = await request2.GetResponseAsync();
                                using (Stream stream2 = response2.GetResponseStream())
                                {
                                    StreamReader reader2 = new StreamReader(stream2, Encoding.UTF8);
                                    String responseString2 = reader2.ReadToEnd();
                                    vocaData jsonArray2 =
                                        JsonConvert.DeserializeObject<vocaData>(responseString2);
                                    String state2;
                                    if (jsonArray2.id.ToString().Equals(jsonArray["collectionId"]))
                                    {
                                        state2 = "SUCCESS";
                                        this.Frame.Navigate(typeof(Vocabulary), jsonArray2);
                                    }
                                    else
                                    {
                                        state2 = "ERROR";
                                    }
                                    //MessageDialog msg = new MessageDialog(jsonArray2["requestMessage"], state2);
                                    //await msg.ShowAsync();
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    else
                    {
                        state = "ERROR";
                    }
                    MessageDialog msg2 = new MessageDialog(jsonArray["requestMessage"], state);
                    await msg2.ShowAsync();
                }
            }
            catch (Exception)
            {
            }
        }
    }

    public class TransmitData
    {
        public int Spec { get; set; } ///// 1 : img   2 : HyperLink
        public String Article { get; set; }
        public String Path { get; set; } ////// img : Image Local Path    HyperLink : HTML Source Code
    }
}
