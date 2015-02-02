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
using System.Text;
using WindowsPreview.Media.Ocr;
using Windows.UI.Popups;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace Wordpedia_window_phone
{
    public sealed partial class Library : Page
    {
        static public bool act_picture = false;
        private SQLiteConnection conn;
        private TransmitData transData;
        private List<vocaData> vocalist;

        public Library()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            tb_search.Width = Window.Current.Bounds.Width - 130;
            lv_Voca.Height = Window.Current.Bounds.Height - 225;

            initialize();

            if (act_picture)
                btn_capture_Click(new object(), new RoutedEventArgs());

        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (conn != null)
                conn.Close();
        }

        private void initialize()
        {
            //////////////////////All Vocabulary Load/////////////////////////
            ////sqlite db 를 열어 SQLvocaData를 받아와 vocaData로 변환 뒤 list에 Add시킨다.////

            vocalist = new List<vocaData>();

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

                    Path = v.Path,
                    Article = v.Article,
                };
                vocalist.Add(data);
            }

            lv_Voca.ItemsSource = vocalist;
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

                    transData = new TransmitData();
                    transData.Spec = 1;
                    /////////////////////////OCRActivate//////////////////////////
                    await OCRActivate(bitmap);
                    return;
                }
            }
        }
        private async Task OCRActivate(WriteableBitmap bitmap)
        {
            //////////////////////Check Image Size//////////////////////
            if (bitmap.PixelHeight < 40 ||
                bitmap.PixelHeight > 2600 ||
                bitmap.PixelWidth < 40 ||
                bitmap.PixelWidth > 2600)
            {
                String Text = "Image size is not supported." +
                                    Environment.NewLine +
                                    "Loaded image size is " + bitmap.PixelWidth + "x" + bitmap.PixelHeight + "." +
                                    Environment.NewLine +
                                    "Supported image dimensions are between 40 and 2600 pixels.";
                //MessageDialog msgbox = new MessageDialog(Text);
                //await msgbox.ShowAsync();

                return;
            }
            ////////////////////Image Copy to Local Folder////////////////////
            ///////////will add try catch///////

            StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync("wordpedia_" + new Random().Next(0, 10000).ToString() + ".img", CreationCollisionOption.GenerateUniqueName);
            await FileIO.WriteBufferAsync(file, bitmap.PixelBuffer);

            transData.Path = file.Path;

            ///////////////////////Activate OCR////////////////////////////

            OcrEngine ocrEngine = new OcrEngine(OcrLanguage.English);
            var ocrResult = await ocrEngine.RecognizeAsync((uint)bitmap.PixelHeight, (uint)bitmap.PixelWidth, bitmap.PixelBuffer.ToArray());

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
                transData.Article = ocr.ToString();
                this.Frame.Navigate(typeof(CreateVocabulary), transData);
            }
            else
            {
                ///////////No Vocabulary/////////////
                var dialog = new MessageDialog("No Vocabulary in Picture");
                await dialog.ShowAsync();
            }
        }

        private void btn_search_Click(object sender, RoutedEventArgs e)
        {
            //////////////////Search ///////////////////
            String query = tb_search.Text;
            List<vocaData> search_vocalist = new List<vocaData>();
            foreach (vocaData v in vocalist)
            {
                if (v.Title.Contains(query)
                    || v.Date.ToString().Contains(query))
                    search_vocalist.Add(v);
            }

            lv_Voca.ItemsSource = search_vocalist;
        }

    }
}
