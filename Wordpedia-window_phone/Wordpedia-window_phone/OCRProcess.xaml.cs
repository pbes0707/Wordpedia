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
using System.Threading.Tasks;
using Newtonsoft.Json;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Wordpedia_window_phone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OCRProcess : Page
    {
        private SQLiteConnection conn;
        private OcrEngine ocrEngine;

        public OCRProcess()
        {
            this.InitializeComponent();
            ocrEngine = new OcrEngine(OcrLanguage.English);
            this.Tag = "load";
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            WriteableBitmap img = (WriteableBitmap)e.Parameter;
            //////////////////////////OCR Activate///////////////////////////
            OCRActivate(img);
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (conn != null)
                conn.Close();
        }

        private async void OCRActivate(WriteableBitmap bitmap)
        {
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

                return;
            }

            // This main API call to extract text from image.
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
            }
            ////////////////////////////String Split/////////////////////////////
            string lowerString = ocr.ToString().ToLower();
            string[] separators = { ",", ".", "!", "?", ";", ":", " ", "\r", "\n", "\t" };
            string[] words = lowerString.Split(separators, StringSplitOptions.RemoveEmptyEntries);

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
            ////////////////////Image Copy to Local Folder////////////////////
            StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync("wordpedia_" + new Random().Next(0,10000).ToString() + ".img", CreationCollisionOption.GenerateUniqueName);
            await FileIO.WriteBufferAsync(file, bitmap.PixelBuffer);

            //////////////////////Voca Data Create//////////////////////////
            vocaData vocadata = new vocaData()
            {
                Kind = 1,
                Title = ocr.ToString().Substring(0, 15),
                Date = DateTime.Now,
                Translate = "en-kr",
                Words = wordList,

                Img = file.Path,
                Article = lowerString,
                Href = null
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

                Img = vocadata.Img,
                Article = vocadata.Article,
                Href = vocadata.Href
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
}
