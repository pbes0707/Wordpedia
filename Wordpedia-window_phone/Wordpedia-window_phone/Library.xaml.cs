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

namespace Wordpedia_window_phone
{
    public sealed partial class Library : Page
    {
        public Library()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            tb_search.Width = Window.Current.Bounds.Width - 130;
            lv_CollectionList.Height = Window.Current.Bounds.Height - 225;

            initialize();
        }

        private void initialize()
        {
            List<collectionData> list = new List<collectionData>();

            //////////////////////All Vocabulary Load/////////////////////////
            for (int i = 0; i < 10; i++)
            {
                list.Add(new collectionData()
                {
                    Title = "Title : " + i.ToString(),
                    Date = i.ToString(),
                    Translate = "translate : " + i.ToString()
                });
            }
            /////////////////////////////////////////////////////////////////

            lv_CollectionList.ItemsSource = list;
        }
    }
}
