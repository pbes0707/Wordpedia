using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WindowsPreview.Media.Ocr;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Wordpedia_windows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<navigation_item> navigation_list;
        private List<vocaData> voca_list;
        private navigation_item prev_navItem;
        private String[] words;
        private WriteableBitmap bitmap;
        private bool isLogin = false;
        private userData _userData;


        public MainPage()
        {
            this.InitializeComponent();

            initialize();

        }
        public void initialize()
        {
            /////////////////////Navigation Initialize////////////////////
            navigation_list = new List<navigation_item>();
            prev_navItem = new navigation_item();
            voca_list = new List<vocaData>();
            prev_navItem.Id = 0;

            navigation_item v = new navigation_item();
            v.Id = 1;
            v.Text = "User Id";
            v.ImgSrc = "Assets/Navigation/user_icon.png";
            navigation_list.Add(v.Copy());

            v.Id = 2;
            v.Text = "Library";
            v.ImgSrc = "Assets/Navigation/library_icon.png";
            navigation_list.Add(v.Copy());

            v.Id = 3;
            v.Text = "Stats";
            v.ImgSrc = "Assets/Navigation/stats_icon.png";
            navigation_list.Add(v.Copy());

            v.Id = 4;
            v.Text = "Surf";
            v.ImgSrc = "Assets/Navigation/surf_icon.png";
            navigation_list.Add(v.Copy());

            lv_navigation.ItemsSource = navigation_list;

            String[] _language = {"English","Czech","Danish","German","Greek","English","Spanish",
                                "Finnish","French","Hungarian","Italian","Japanese","Korean","Dutch",
                                "Norwegian","Polish","Russian","Swedish","Turkish","ChineseSimplified","Portuguese"};

            cbx_createVoca_ocrlanguage.ItemsSource = _language;
            cbx_createVoca_ocrlanguage.SelectedItem = "English";


            /////////////////Design Initialize///////////////////////////
            Grid_Navigation.Width = 300;
            Grid_Navigation.Height = Window.Current.Bounds.Height;

            Grid_UserEmail.Width = Window.Current.Bounds.Width - 300;
            Grid_UserEmail.Height = Window.Current.Bounds.Height;

            cbx_createVoca_language.ItemsSource = LanguageCode.getLanguageName();
            cbx_createVoca_language.SelectedItem = "Korean";

            //////////////////App Start Login//////////////////////////////

            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            if (localSettings.Values.ContainsKey("id") == true && localSettings.Values["id"].ToString() != "0")
            {
                String _idObject = localSettings.Values["id"].ToString();
                String _passwdObject = localSettings.Values["passwd"].ToString();
                String _tokenObject = localSettings.Values["token"].ToString();
                loginFunction(_idObject, _passwdObject, false);
            }

        }

        private void lv_navigation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lv_navigation.SelectedItems.Count == 0) return;
            navigation_item v = lv_navigation.SelectedItems[0] as navigation_item;

            if (prev_navItem.Id != v.Id)
            {
                switch (v.Id)
                {
                    case 1:
                        ChangeGridVisibility(Grid_UserEmail, Visibility.Visible, true);
                        if (isLogin) //로그인이 되 있을 경우
                        {
                            ChangeGridVisibility(Grid_UserEmail_isLogin, Visibility.Visible);
                        }
                        else // 로그인이 되있지 않을 경우
                        {
                            ChangeGridVisibility(Grid_UserEmail_LoginRegist, Visibility.Visible);
                        }
                        break;
                    case 2:
                        if (isLogin == false) break;
                        loadLibraryList();

                        ChangeGridVisibility(Grid_Library, Visibility.Visible, true);
                        ChangeGridVisibility(Grid_Library_list, Visibility.Visible);
                        break;
                    case 3:
                        if (isLogin == false) break;
                        break;
                    case 4:
                        if (isLogin == false) break;
                        break;
                }
                prev_navItem = v.Copy();
            }
            lv_navigation.SelectedItem = null;
        }
        void ChangeGridVisibility(Grid grid, Visibility visible, bool other = false)
        {
            if (other == true)
            {
                Grid_UserEmail.Visibility = Visibility.Collapsed;
                Grid_Library.Visibility = Visibility.Collapsed;

                Grid_UserEmail_LoginRegist.Visibility = Visibility.Collapsed;
                Grid_UserEmail_isLogin.Visibility = Visibility.Collapsed;
                Grid_Library_list.Visibility = Visibility.Collapsed;
                Grid_Library_voca.Visibility = Visibility.Collapsed;
                Grid_Library_createVoca.Visibility = Visibility.Collapsed;
                //자신 이외의 다른 그리드를 모두 비활성화
            }
            grid.Visibility = visible;
        }
        class navigation_item
        {
            public int Id { get; set; }
            public String Text { get; set; }
            public String ImgSrc { get; set; }

            public navigation_item Copy()
            {
                navigation_item v = new navigation_item();
                v.Id = Id;
                v.Text = Text;
                v.ImgSrc = ImgSrc;
                return v;
            }
        }

        ///////////////Regist//////////////////
        private void btn_loginregist_signin_Click(object sender, RoutedEventArgs e)
        {
            registFunction(tbx_loginregist_ID.Text, tbx_loginregist_passwd.Password);
        }
        private void btn_loginregist_confirm_Click(object sender, RoutedEventArgs e)
        {
            loginFunction(tbx_loginregist_ID.Text, tbx_loginregist_passwd.Password);
        }
        private void btn_isLogin_signout_Click(object sender, RoutedEventArgs e)
        {

            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            localSettings.Values.Remove("id");
            localSettings.Values.Remove("passwd");
            localSettings.Values.Remove("token");

            ChangeGridVisibility(Grid_UserEmail_isLogin, Visibility.Collapsed);
            ChangeGridVisibility(Grid_UserEmail_LoginRegist, Visibility.Visible);


            foreach (navigation_item v in navigation_list)
                if (v.Id == 1)
                    v.Text = "User Id";

            lv_navigation.ItemsSource = null;
            lv_navigation.ItemsSource = navigation_list;

            isLogin = false;
        }

        private async void loginFunction(String _ID, String _pw, bool show = true)
        {
            string url = "http://wordpedia.herokuapp.com/login?id=" + _ID + "&pw=" + _pw;
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
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
                    if (jsonArray.ContainsKey("requestMessage") == true && jsonArray["requestMessage"].Equals("로그인에 성공했습니다."))
                    {
                        state = "SUCCESS";
                        String _id = _ID;
                        String _passwd = _pw;
                        String _token = jsonArray["token"].ToString();

                        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

                        localSettings.Values["id"] = _id;
                        localSettings.Values["passwd"] = _passwd;
                        localSettings.Values["token"] = _token;

                        tb_isLogin_welcome.Text = "Welcome to " + _id;

                        ChangeGridVisibility(Grid_UserEmail_LoginRegist, Visibility.Collapsed);
                        if (show == true) ChangeGridVisibility(Grid_UserEmail_isLogin, Visibility.Visible);

                        tbx_loginregist_ID.Text = "";
                        tbx_loginregist_passwd.Password = "";

                        isLogin = true;
                    }
                    else
                    {
                        state = "ERROR";

                    }
                    MessageDialog msg = new MessageDialog(jsonArray["requestMessage"].ToString(), state);
                    if (show == true) await msg.ShowAsync();

                    foreach (navigation_item v in navigation_list)
                        if (v.Id == 1)
                            v.Text = _ID;

                    lv_navigation.ItemsSource = null;
                    lv_navigation.ItemsSource = navigation_list;

                }
            }
            catch (Exception)
            {
            }
        }
        private async void registFunction(String _ID, String _pw)
        {
            string url = "http://wordpedia.herokuapp.com/signup?id=" + _ID + "&pw=" + _pw;
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
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
                    if (jsonArray["requestMessage"].Equals("회원가입에 성공했습니다."))
                    {
                        state = "SUCCESS";

                        tbx_loginregist_ID.Text = "";
                        tbx_loginregist_passwd.Password = "";
                    }
                    else
                    {
                        state = "ERROR";
                    }
                    MessageDialog msg = new MessageDialog(jsonArray["requestMessage"], state);
                    await msg.ShowAsync();
                }
            }
            catch (Exception)
            {
            }
        }


        ///////////////Library////////////////////
        private async void loadLibraryList()
        {
            string url = "http://wordpedia.herokuapp.com/get/collection/user";
            System.Net.WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            request.Headers["token"] = localSettings.Values["token"].ToString();

            try
            {
                WebResponse response = await request.GetResponseAsync();
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    String responseString = reader.ReadToEnd();
                    _userData =
                        JsonConvert.DeserializeObject<userData>(responseString);
                    lv_voca.ItemsSource = _userData.collections;
                    voca_list = _userData.collections;
                }
            }
            catch (Exception)
            {

            }
        }
        private async void createVocabulrary()
        {

            string url = "http://wordpedia.herokuapp.com/collection/create?";
            System.Net.WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            request.Headers["token"] = localSettings.Values["token"].ToString();

            try
            {
                WebResponse response = await request.GetResponseAsync();
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    String responseString = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
            }
        }

        private void tbx_loginregist_ID_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
        private void sb_search_SuggestionsRequested(SearchBox sender, SearchBoxSuggestionsRequestedEventArgs args)
        {
        }
        private void sb_search_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            List<vocaData> temp_voca = new List<vocaData>();
            string query = args.QueryText;
            foreach (vocaData v in voca_list)
                if (v.createDate.ToLower().Contains(query.ToLower()) || v.creator.ToLower().Contains(query.ToLower())
                    || v.title.ToLower().Contains(query.ToLower()) || v.to.ToLower().Contains(query.ToLower()))
                    temp_voca.Add(v);
            lv_voca.ItemsSource = temp_voca;
        }
        private void lv_voca_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lv_voca.SelectedItems.Count == 0) return;
            vocaData v = lv_voca.SelectedItems[0] as vocaData;

            ChangeGridVisibility(Grid_Library_list, Visibility.Collapsed);
            ChangeGridVisibility(Grid_Library_voca, Visibility.Visible);

            tb_Library_title.Text = v.title;
            tb_Library_createDate.Text = v.createDate;
            tb_Library_translate.Text = v.fullTranslate;

            List<Word> temp_list = new List<Word>();
            foreach (Word w in v.wordList)
                if (!w.translateWord.Equals(w.word))
                    temp_list.Add(w);

            lv_Library_word.ItemsSource = temp_list;
        }
        private async void btn_capture_Click(object sender, RoutedEventArgs e)
        {
            DataPackageView dataPackageView = Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Bitmap))
            {
                ////////////원문 언어가 무엇인지 팝업을 띄워주어야 한다////////////
                int[] _code = { 1028, 1029, 1030, 1031, 1032, 1033, 1034, 1035, 1036, 1038, 1040, 1041, 1042, 1043, 1044, 1045, 1049, 1053, 1055, 2052, 2070 };
                IRandomAccessStreamReference imageReceived = null;
                try
                {
                    imageReceived = await dataPackageView.GetBitmapAsync();
                }
                catch (Exception ex)
                {
                }

                if (imageReceived != null)
                {
                    using (IRandomAccessStream imageStream = await imageReceived.OpenReadAsync())
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.SetSource(imageStream);

                        IRandomAccessStream writeBitmapStream = await imageReceived.OpenReadAsync();
                        bitmap = new WriteableBitmap(
                            bitmapImage.PixelWidth, bitmapImage.PixelHeight);
                        bitmap.SetSource(writeBitmapStream);

                        ChangeGridVisibility(Grid_Library_list, Visibility.Collapsed);
                        ChangeGridVisibility(Grid_Library_createVoca, Visibility.Visible);

                    }
                }
            }
            else
            {
                var dialog = new MessageDialog("Your Clipboard data is not Image", "Error");
                await dialog.ShowAsync();
            }
            //this.sex.Source = await CreateBitmapFromElement(this.Parent as FrameworkElement);
        }


        /////////////////////Create Vocabulary//////////////////
        private async void btn_create_vocabulary_Click(object sender, RoutedEventArgs e)
        {

            OcrLanguage olc = (OcrLanguage)Enum.Parse(typeof(OcrLanguage), cbx_createVoca_ocrlanguage.SelectedItem as String);
            OcrEngine ocrEngine = new OcrEngine(olc);
            var ocrResult = await ocrEngine.RecognizeAsync(
                (uint)bitmap.PixelHeight,
                (uint)bitmap.PixelWidth,
                bitmap.PixelBuffer.ToArray());

            StringBuilder ocr = new StringBuilder();
            if (ocrResult.Lines != null)
            {
                foreach (var line in ocrResult.Lines)
                {
                    string newLine = string.Empty;
                    foreach (var word in line.Words)
                    {
                        newLine = newLine + word.Text + " ";
                    }
                    ocr.AppendLine(newLine);
                }
                String Article = ocr.ToString();
                String transArticle = Article.ToLower();
                /*Regex regex = new Regex("<[^>]*>", RegexOptions.IgnoreCase);
                String result = regex.Replace(htmlContent, "");
                result = result.Replace("\n", "");*/
                transArticle = Regex.Replace(transArticle, @"[^a-zA-Z0-9가-힣]", ",", RegexOptions.IgnoreCase);
                String[] separators = { "," };
                words = transArticle.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                if (words[0] != null)
                {
                    ChangeGridVisibility(Grid_Library_list, Visibility.Collapsed);
                    ChangeGridVisibility(Grid_Library_createVoca, Visibility.Visible);
                }
                else
                {
                    var dialog = new MessageDialog("No Vocabulary in Article", "ERROR");
                    await dialog.ShowAsync();
                }
            }
            else
            {
                ///////////No Vocabulary/////////////
                var dialog = new MessageDialog("No Vocabulary in Picture", "ERROR");
                await dialog.ShowAsync();
            }
            String title = tbx_createVoca_title.Text;
            String to = LanguageCode.getLanguageCode()[cbx_createVoca_language.SelectedIndex];
            List<String> fixed_words = new List<String>();
            foreach(String v in words)
            {
                if (v.Length < 2) continue;
                bool flag = false;
                foreach(String w in fixed_words)
                    if( v.Equals(w) == true)
                        flag = true;
                if (flag == false)
                    fixed_words.Add(v);
            }
            String query = fixed_words[0];
            for(int i = 1 ;i<fixed_words.Count ; i++)
            {
                query += "&w=" + fixed_words[i];
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
                        tbx_createVoca_title.Text = "";

                        loadLibraryList();
                        ChangeGridVisibility(Grid_Library_createVoca, Visibility.Collapsed);
                        ChangeGridVisibility(Grid_Library_list, Visibility.Visible);
                    }
                    else
                    {
                        state = "ERROR";
                    }
                    MessageDialog msg = new MessageDialog(jsonArray["requestMessage"], state);
                    await msg.ShowAsync();
                }
            }
            catch (Exception)
            {
            }
        }
        private void cbx_createVoca_language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void lv_Library_word_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lv_Library_word.SelectedItems.Count == 0) return;
            Word v = lv_Library_word.SelectedItems[0] as Word;

            String uri = "http://translate.google.com/translate_tts?tl=en&q=" + v.word;
            if(me_music.Source != new Uri(uri))me_music.Source = new Uri(uri);
            else me_music.Play();

            lv_Library_word.SelectedItem = null;
        }

    }
}