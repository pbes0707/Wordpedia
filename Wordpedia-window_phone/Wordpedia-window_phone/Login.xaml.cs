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
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Wordpedia_window_phone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Login : Page
    {
        public Login()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            loginFunction(tbx_ID.Text, tbx_pw.Password);
        }

        private async void loginFunction(String _ID, String _pw)
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

                        tbx_ID.Text = "";
                        tbx_pw.Password = "";

                        this.Frame.Navigate(typeof(Library));
                    }
                    else
                    {
                        state = "ERROR";

                    }
                    MessageDialog msg = new MessageDialog(jsonArray["requestMessage"].ToString(), state);
                    await msg.ShowAsync();

                }
            }
            catch (Exception)
            {
            }
        }
    }
}
