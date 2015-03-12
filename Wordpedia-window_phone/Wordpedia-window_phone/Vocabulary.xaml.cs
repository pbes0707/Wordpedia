using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SQLite;
using Windows.Storage;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Phone.UI.Input;

namespace Wordpedia_window_phone
{
    public sealed partial class Vocabulary : Page
    {
        public Vocabulary()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            vocaData t = (vocaData)e.Parameter;
            
            initialize(t);
        }


        private void initialize(vocaData t)
        {
            tb_Title.Text = t.title;

            var show_list = new List<Word>();
            foreach (Word v in t.wordList)
            {
                if( v.word != "" 
                    && v.word != v.translateWord)
                {
                    show_list.Add(v);
                }
            }
            lv_words.ItemsSource = show_list;

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                return;
            }

            if (frame.CanGoBack)
            {
                frame.Navigate(typeof(Library));
                e.Handled = true;
            }
        }

        private void lv_words_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lv_words.SelectedItems.Count == 0) return;
            Word v = lv_words.SelectedItems[0] as Word;

            ////////////////Google TTS API//////////////////
            String mp3Url = "http://translate.google.com/translate_tts?tl=en&q=";
            mp3Url += v.word;
            media.Source = new Uri(mp3Url);

            lv_words.SelectedItem = null;
        }

        private void media_MediaEnded(object sender, RoutedEventArgs e)
        {
        }

    }
}
