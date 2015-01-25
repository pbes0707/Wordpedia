using System;
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
using Newtonsoft.Json;
using Windows.Phone.UI.Input;

namespace Wordpedia_window_phone
{
    public sealed partial class Library : Page
    {
        private SQLiteConnection conn;

        public Library()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            tb_search.Width = Window.Current.Bounds.Width - 130;
            lv_Voca.Height = Window.Current.Bounds.Height - 225;

            initialize();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (conn != null)
                conn.Close();
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        private void initialize()
        {
            //////////////////////All Vocabulary Load/////////////////////////
            ////sqlite db 를 열어 SQLvocaData를 받아와 vocaData로 변환 뒤 list에 Add시킨다.////

            List<vocaData> vocalist = new List<vocaData>();


            string strConn = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "Vocabulary.sqlite")); ;

            conn = new SQLiteConnection(strConn);
            conn.CreateTable<SQLvocaData>();
            List<SQLvocaData> SQLvocalist = conn.Table<SQLvocaData>().ToList<SQLvocaData>();

            foreach(SQLvocaData v in SQLvocalist)
            {
                vocaData data = new vocaData()
                {
                    Kind = v.Kind,
                    Title = v.Title,
                    Date = v.Date,
                    Translate = v.Translate,
                    Words = JsonConvert.DeserializeObject<List<wordData>>(v.JsonWords),

                    Img = v.Img,
                    Article = v.Article,
                    Href = v.Href
                };

                vocalist.Add(data);
            }

            lv_Voca.ItemsSource = vocalist;
            /////////////////////////////////////////////////////////////////

        }

        private void lv_CollectionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lv_Voca.SelectedItems.Count == 0) return;
            vocaData v = lv_Voca.SelectedItems[0] as vocaData;

            /////////////////////단어장 오픈/////////////////////
           
            this.Frame.Navigate(typeof(Vocabulary), v);
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

                ImageProperties imgProp = await file.Properties.GetImagePropertiesAsync();
                using (var imgStream = await file.OpenAsync(FileAccessMode.Read))
                {
                    WriteableBitmap bitmap = new WriteableBitmap((int)imgProp.Width, (int)imgProp.Height);
                    bitmap.SetSource(imgStream);

                    this.Frame.Navigate(typeof(OCRProcess), bitmap);
                }

            }
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
                frame.GoBack();
                e.Handled = true;
            }
        }


    }
}
