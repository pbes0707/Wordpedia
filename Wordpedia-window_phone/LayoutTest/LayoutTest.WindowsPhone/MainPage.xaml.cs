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

// 빈 페이지 항목 템플릿에 대한 설명은 http://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace LayoutTest
{
    /// <summary>
    /// 자체에서 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        TaskCompletionSource<StorageFile> completionSource;

        WriteableBitmap writimage;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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

                PickImage(openPicker);
            }
            catch (Exception)
            {

            }
        }
        private async void PickImage(FileOpenPicker openPicker)
        {
            try
            {
                this.completionSource = new TaskCompletionSource<StorageFile>();

                openPicker.PickSingleFileAndContinue();

                StorageFile file = await this.completionSource.Task;

                if (file != null)
                {
                    img.Source = await MakeImage(file);
                }
            }
            catch (Exception)
            { }
        }
        
        async Task<ImageSource> MakeImage(StorageFile file)
        {
            BitmapImage bitmapImage = null;

            if (file != null)
            {
                bitmapImage = new BitmapImage();

                using (var stream = await file.OpenReadAsync())
                {
                    await bitmapImage.SetSourceAsync(stream);
                }
            }
            return (bitmapImage);
        }
    }
}
