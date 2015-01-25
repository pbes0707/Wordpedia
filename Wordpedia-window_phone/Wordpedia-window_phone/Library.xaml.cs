﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Media.Capture;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using SQLite;
using Windows.Storage.Streams;
using Windows.Storage.FileProperties;

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
            lv_Voca.Height = Window.Current.Bounds.Height - 225;

            initialize();
        }

        private void initialize()
        {
            List<vocaData> list = new List<vocaData>();

            //////////////////////All Vocabulary Load/////////////////////////
            for (int i = 0; i < 10; i++)
            {
                list.Add(new vocaData()
                {
                    Title = "Title : " + i.ToString(),
                    Date = i.ToString(),
                    Translate = "translate : " + i.ToString()
                });
            }
            /////////////////////////////////////////////////////////////////

            lv_Voca.ItemsSource = list;
        }

        private void lv_CollectionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lv_Voca.SelectedItems.Count == 0) return;
            vocaData v = lv_Voca.SelectedItems[0] as vocaData;

            /////////////////////단어장 오픈/////////////////////
            kind t = new kind();
            t.Spec = 0;
            this.Frame.Navigate(typeof(Vocabulary), t);
        }

        private void btn_capture_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openPicker = new FileOpenPicker
                {
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                    ViewMode = PickerViewMode.Thumbnail
                };

                // Filter to include a sample subset of file types.
                openPicker.FileTypeFilter.Clear();
                openPicker.FileTypeFilter.Add(".bmp");
                openPicker.FileTypeFilter.Add(".png");
                openPicker.FileTypeFilter.Add(".jpeg");
                openPicker.FileTypeFilter.Add(".jpg");

                openPicker.PickSingleFileAndContinue();
            }
            catch (Exception)
            {

            }
        }

        public async void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            if (args.Files.Count > 0)
            {
                StorageFile file = args.Files[0];

                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    ImageProperties props = await file.Properties.GetImagePropertiesAsync();
                    int height = (int)props.Width;
                    int width = (int)props.Width;
                    WriteableBitmap bitmap = new WriteableBitmap(height, width);
                    bitmap.SetSource(fileStream);

                    this.Frame.Navigate(typeof(OCRProcess), bitmap);
                }

            }
        }

    }
}
