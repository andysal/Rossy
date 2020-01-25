using Rossy.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Rossy.App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private AppConfig AppConfiguration { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            AppConfiguration = new AppConfig();

            txtUtterance.Text = "what's up?";
            txtFilePath.Text = @"C:\Temp\WP_20170520_17_30_04_Rich.jpg";
        }

        private async void btnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            var config = new AppConfig();
            string utterance = txtUtterance.Text;
            string filePath = txtFilePath.Text;

            using (var fileStream = File.Open(filePath, FileMode.Open))
            {
                var storageManager = new Storage(config.StorageConfig);
                var (fileName, blobUrl) = storageManager.UploadFile(fileStream);
                fileStream.Close();
                fileStream.Dispose();

                var rosetta = new Rosetta(config.RosettaConfig);
                var intent = rosetta.GuessIntent(utterance);
                var analyzer = new Sherlock(config.SherlockConfig);
                switch (intent)
                {
                    case "People":
                        analyzer.People(blobUrl);
                        break;
                    case "FullScan":
                    default:
                        analyzer.FullScan(blobUrl);
                        break;
                }
                storageManager.DeleteFile(fileName);
            }
        }

        private async void btnPickFile_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker filePicker = new FileOpenPicker();
            filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            filePicker.ViewMode = PickerViewMode.Thumbnail;

            filePicker.FileTypeFilter.Clear();
            filePicker.FileTypeFilter.Add(".jpeg"); filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".png"); filePicker.FileTypeFilter.Add(".gif");

            StorageFile file = await filePicker.PickSingleFileAsync();
            if (null != file)
            {
                var (fileName, blobUrl) = await UploadPicture(file);
            }
        }

        private async void btnTakePicture_Click(object sender, RoutedEventArgs e)
        {
            CameraCaptureUI dialog = new CameraCaptureUI();
            StorageFile file = await dialog.CaptureFileAsync(CameraCaptureUIMode.Photo);
            var (fileName, blobUrl) = await UploadPicture(file);
        }

        private async Task<(string, string)> UploadPicture(StorageFile file)
        {
            var randomAccessStream = await file.OpenReadAsync();
            using (Stream stream = randomAccessStream.AsStreamForRead())
            {
                var storageManager = new Storage(AppConfiguration.StorageConfig);
                var (fileName, blobUrl) = storageManager.UploadFile(stream);
                return (fileName, blobUrl);
            }
        }
    }
}
