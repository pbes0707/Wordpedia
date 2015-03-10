using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Wordpedia_windows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<navigation_item> navigation_list;
        private navigation_item prev_navItem;
        private bool isLogin = false;
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
            prev_navItem.Id = 0;

            navigation_item v = new navigation_item();
            v.Id = 1;
            v.Text = "User Email";
            navigation_list.Add(v.Copy());

            v.Id = 2;
            v.Text = "Library";
            navigation_list.Add(v.Copy());

            v.Id = 3;
            v.Text = "Stats";
            navigation_list.Add(v.Copy());

            v.Id = 4;
            v.Text = "Surf";
            navigation_list.Add(v.Copy());

            lv_navigation.ItemsSource = navigation_list;

            /////////////////Design Initialize///////////////////////////
            Grid_Navigation.Width = 300;
            Grid_Navigation.Height = Window.Current.Bounds.Height;

            Grid_UserEmail.Width = Window.Current.Bounds.Width - 300;
            Grid_UserEmail.Height = Window.Current.Bounds.Height;

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
                            ChangeGridVisibility(Grid_UserEmail_regist, Visibility.Visible);
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
        }
        void ChangeGridVisibility(Grid grid, Visibility visible, bool other = false)
        {
            if (other == true)
            {
                Grid_UserEmail.Visibility = Visibility.Collapsed;
                Grid_Library.Visibility = Visibility.Collapsed;

                Grid_UserEmail_regist.Visibility = Visibility.Collapsed;
                Grid_UserEmail_login.Visibility = Visibility.Collapsed;
                Grid_UserEmail_isLogin.Visibility = Visibility.Collapsed;
                Grid_Library_list.Visibility = Visibility.Collapsed;
                //자신 이외의 다른 그리드를 모두 비활성화
            }
            grid.Visibility = visible;
        }
        class navigation_item
        {
            public int Id { get; set; }
            public String Text { get; set; }

            public navigation_item Copy()
            {
                navigation_item v = new navigation_item();
                v.Id = Id;
                v.Text = Text;
                return v;
            }
        }


        ///////////////Regist//////////////////
        private void btn_regist_signin_Click(object sender, RoutedEventArgs e)
        {
            registFunction(tbx_regist_ID.Text, tbx_regist_passwd.Password);
        }
        private void btn_regist_login_Click(object sender, RoutedEventArgs e)
        {
            ChangeGridVisibility(Grid_UserEmail_regist, Visibility.Collapsed);
            ChangeGridVisibility(Grid_UserEmail_login, Visibility.Visible);
        }

        ///////////////Login///////////////////
        private void btn_login_confirm_Click(object sender, RoutedEventArgs e)
        {
            loginFunction(tbx_login_ID.Text, tbx_login_passwd.Password);
        }
        private void btn_login_regist_Click(object sender, RoutedEventArgs e)
        {
            ChangeGridVisibility(Grid_UserEmail_login, Visibility.Collapsed);
            ChangeGridVisibility(Grid_UserEmail_regist, Visibility.Visible);
        }

        //////////// isLogin//////////////////////
        private void btn_isLogin_signout_Click(object sender, RoutedEventArgs e)
        {

            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            localSettings.Values.Remove("id");
            localSettings.Values.Remove("passwd");
            localSettings.Values.Remove("token");

            ChangeGridVisibility(Grid_UserEmail_isLogin, Visibility.Collapsed);
            ChangeGridVisibility(Grid_UserEmail_login, Visibility.Visible);

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

                        ChangeGridVisibility(Grid_UserEmail_login, Visibility.Collapsed);
                        if(show==true) ChangeGridVisibility(Grid_UserEmail_isLogin, Visibility.Visible);

                        tbx_login_ID.Text = "";
                        tbx_login_passwd.Password = "";

                        isLogin = true;
                    }
                    else
                    {
                        state = "ERROR";

                    }
                    MessageDialog msg = new MessageDialog(jsonArray["requestMessage"].ToString(), state);
                    if(show==true) await msg.ShowAsync();
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

                        tbx_regist_ID.Text = "";
                        tbx_regist_passwd.Password = "";
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
                    userData _userData =
                        JsonConvert.DeserializeObject<userData>(responseString);
                }
            }
            catch(Exception)
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
    }
}