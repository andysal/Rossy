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
            txtFilePath.Text = "";
        }

        private async void btnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            var config = new AppConfig();
            string utterance = txtUtterance.Text;

            string blobUrl;
            if (string.IsNullOrWhiteSpace(txtFilePath.Text))
            {
                CameraCaptureUI dialog = new CameraCaptureUI();
                StorageFile file = await dialog.CaptureFileAsync(CameraCaptureUIMode.Photo);
                blobUrl = await UploadPicture(file);
            }
            else
            {
                blobUrl = txtFilePath.Text;
            }

            var rosetta = new Rosetta(config.RosettaConfig);
            var intent = rosetta.GuessIntent(utterance);
            var analyzer = new Sherlock(config.SherlockConfig);
            var response = string.Empty;
            switch (intent)
            {
                case "People":
                    response = analyzer.People(blobUrl);
                    break;
                case "FullScan":
                default:
                    response = analyzer.FullScan(blobUrl);
                    break;
            }
            txtAnalysisResult.Text = response;
            DeletePicture(blobUrl);
        }

        private async void btnPickFile_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker filePicker = new FileOpenPicker();
            filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            filePicker.ViewMode = PickerViewMode.Thumbnail;

            filePicker.FileTypeFilter.Clear();
            filePicker.FileTypeFilter.Add(".jpeg"); 
            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".png");

            StorageFile file = await filePicker.PickSingleFileAsync();
            if (null != file)
            {
                var blobUrl = await UploadPicture(file);
                txtFilePath.Text = blobUrl;
            }
        }

        private async void btnTakePicture_Click(object sender, RoutedEventArgs e)
        {
            CameraCaptureUI dialog = new CameraCaptureUI();
            StorageFile file = await dialog.CaptureFileAsync(CameraCaptureUIMode.Photo);
            var blobUrl = await UploadPicture(file);
        }

        private async Task<string> UploadPicture(StorageFile file)
        {
            var randomAccessStream = await file.OpenReadAsync();
            using (Stream stream = randomAccessStream.AsStreamForRead())
            {
                var storageManager = new Storage(AppConfiguration.StorageConfig);
                var blobUrl = storageManager.UploadFile(stream);
                return blobUrl;
            }
        }

        private void DeletePicture(string blobUrl)
        {
            var storageManager = new Storage(AppConfiguration.StorageConfig);
            var blobUri = new Uri(blobUrl);
            storageManager.DeleteFile(blobUri);
        }
    }
}
