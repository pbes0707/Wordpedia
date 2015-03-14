using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WinRTScreenshot
{
   /// <summary>
   /// An empty page that can be used on its own or navigated to within a Frame.
   /// </summary>
   public sealed partial class MainPage : Page
   {
      public MainPage()
      {
         this.InitializeComponent();

         btnScreenshot.Click += btnScreenshot_Click;
      }

      async void btnScreenshot_Click(object sender, RoutedEventArgs e)
      {
         var bitmap = await SaveScreenshotAsync(controlsGrid);

         imagePreview.Source = bitmap;
      }

      async Task<RenderTargetBitmap> SaveScreenshotAsync(FrameworkElement uielement)
      {
         var file = await PickSaveImageAsync();

         return await SaveToFileAsync(uielement, file);         
      }

      async Task<StorageFile> PickSaveImageAsync()
      {
         var filePicker = new FileSavePicker();
         filePicker.FileTypeChoices.Add("Bitmap", new List<string>() { ".bmp" });
         filePicker.FileTypeChoices.Add("JPEG format", new List<string>() { ".jpg" });
         filePicker.FileTypeChoices.Add("Compuserve format", new List<string>() { ".gif" });
         filePicker.FileTypeChoices.Add("Portable Network Graphics", new List<string>() { ".png" });
         filePicker.FileTypeChoices.Add("Tagged Image File Format", new List<string>() { ".tif" });
         filePicker.DefaultFileExtension = ".jpg";
         filePicker.SuggestedFileName = "screenshot";
         filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
         filePicker.SettingsIdentifier = "picture picker";
         filePicker.CommitButtonText = "Save picture";

         return await filePicker.PickSaveFileAsync();
      }

      async Task<RenderTargetBitmap> SaveToFileAsync(FrameworkElement uielement, StorageFile file)
      {
         if (file != null)
         {
            CachedFileManager.DeferUpdates(file);

            Guid encoderId = GetBitmapEncoder(file.FileType);

            try
            {
               using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
               {
                  return await CaptureToStreamAsync(uielement, stream, encoderId);
               }
            }
            catch(Exception ex)
            {
               DisplayMessage(ex.Message);
            }

            var status = await CachedFileManager.CompleteUpdatesAsync(file);
         }

         return null;
      }

      Guid GetBitmapEncoder(string fileType)
      {
         Guid encoderId = BitmapEncoder.JpegEncoderId;
         switch (fileType)
         {
            case ".bmp":
               encoderId = BitmapEncoder.BmpEncoderId;
               break;
            case ".gif":
               encoderId = BitmapEncoder.GifEncoderId;
               break;
            case ".png":
               encoderId = BitmapEncoder.PngEncoderId;
               break;
            case ".tif":
               encoderId = BitmapEncoder.TiffEncoderId;
               break;
         }

         return encoderId;
      }

      async Task<RenderTargetBitmap> CaptureToStreamAsync(FrameworkElement uielement, IRandomAccessStream stream, Guid encoderId)
      {
         try
         {
            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(uielement);

            var pixels = await renderTargetBitmap.GetPixelsAsync();

            var logicalDpi = DisplayInformation.GetForCurrentView().LogicalDpi;
            var encoder = await BitmapEncoder.CreateAsync(encoderId, stream);
            encoder.SetPixelData(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Ignore,
                (uint)renderTargetBitmap.PixelWidth,
                (uint)renderTargetBitmap.PixelHeight,
                logicalDpi,
                logicalDpi,
                pixels.ToArray());

            await encoder.FlushAsync();

            return renderTargetBitmap;
         }
         catch (Exception ex)
         {
            DisplayMessage(ex.Message);
         }

         return null;
      }

      async void DisplayMessage(string error)
      {
         var dialog = new MessageDialog(error);

         await dialog.ShowAsync();
      }
   }
}
