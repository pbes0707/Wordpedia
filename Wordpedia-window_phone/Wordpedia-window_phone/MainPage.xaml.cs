using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Streams;
using WindowsPreview.Media.Ocr;
using SQLite;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Wordpedia_window_phone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
           // BitmapImage bm = new BitmapImage(new Uri(@"Assets/back.png", UriKind.RelativeOrAbsolute));
           // background.Source = bm;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            OcrEngine ocrEngine;
            WriteableBitmap bitmap = null;

            /*FileOpenPicker open = new FileOpenPicker();
            open.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            open.ViewMode = PickerViewMode.Thumbnail;

            // Filter to include a sample subset of file types
            open.FileTypeFilter.Clear();
            open.FileTypeFilter.Add(".bmp");
            open.FileTypeFilter.Add(".png");
            open.FileTypeFilter.Add(".jpeg");
            open.FileTypeFilter.Add(".jpg");

            // Open a stream for the selected file
            StorageFile file = await open.PickSingleFileAsync();

            // Ensure a file was selected
            if (file != null)
            {
                // Ensure the stream is disposed once the image is loaded
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    ImageProperties props = await file.Properties.GetImagePropertiesAsync();
                    int height = (int)props.Width;
                    int width = (int)props.Width;
                    bitmap = new WriteableBitmap(height, width);
                    bitmap.SetSource(fileStream);
                    image.Source = bitmap;
                }
            }*/

            //////////////////Image Select//////////////////////
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/sample.png"));
            if (file != null)
            {
                // Ensure the stream is disposed once the image is loaded
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    ImageProperties props = await file.Properties.GetImagePropertiesAsync();
                    int height = (int)props.Width;
                    int width = (int)props.Width;
                    bitmap = new WriteableBitmap(height, width);
                    bitmap.SetSource(fileStream);
                    image.Source = bitmap;
                }
            }
            byte[] mbuffer = bitmap.PixelBuffer.ToArray();//SeparateImage(bitmap);

            /////////////////////OCR Activate///////////////////////
            ocrEngine = new OcrEngine(OcrLanguage.English);
            var ocrResult = await ocrEngine.RecognizeAsync(
                (uint)bitmap.PixelHeight,
                (uint)bitmap.PixelWidth,
                mbuffer);
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
                textBlock.Text = ocr.ToString();
            }
            ////////////////////////////String Split/////////////////////////////
            string lowerString = ocr.ToString().ToLower();
            string[] separators = { ",", ".", "!", "?", ";", ":", " ", "\r", "\n" };
            string[] words = lowerString.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            ///////////////////////////List Create////////////////////////////////
            List<wordData> wordList = new List<wordData>();
            foreach(string v in words)
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
                    wordList.Add(new wordData(v));
            }
            //////////////////////Sqlite Table Create///////////////////////
        }

        private byte[][] SeparateImage(WriteableBitmap bitmap)
        {
            int Length = 0, temp = 0;
            byte[][] mbuffer = null;
            foreach(byte v in bitmap.PixelBuffer.ToArray())
            {
                if (temp == 2600)
                {
                    ++Length;
                    temp = 0;
                }
                mbuffer[Length][temp] = v;
                ++temp;
            }
            return mbuffer;
        }
    }
}
