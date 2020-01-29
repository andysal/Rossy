using Microsoft.CognitiveServices.Speech;
using Rossy.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Media.Playback;
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
        private Configuration AppConfiguration { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            AppConfiguration = new AppConfig().GetConfig();

            txtUtterance.Text = "what's up?";
            txtFilePath.Text = "";


        }

        private async void btnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            string blobUrl;
            if(!string.IsNullOrWhiteSpace(txtFilePath.Text))
            {
                blobUrl = txtFilePath.Text;
            }
            else
            {
                CameraCaptureUI dialog = new CameraCaptureUI();
                StorageFile file = await dialog.CaptureFileAsync(CameraCaptureUIMode.Photo);
                blobUrl = await UploadPicture(file);
            }

            string utterance;
            if(!string.IsNullOrWhiteSpace(txtUtterance.Text))
                utterance= txtUtterance.Text;
            else
            {
                utterance = "what's up?";               
            }
            imgPhoto.Source = new BitmapImage(new Uri(txtFilePath.Text));

            var analyzer = new Sherlock(AppConfiguration);
            Sherlock.AnalysisResult response = analyzer.Analyze(blobUrl, utterance);
           
            var modem = new Modem(AppConfiguration.ModemConfig);
            var result = modem.ProduceSpeech(response.Result);
            Play(result);

            txtAnalysisResult.Text = response.Log;
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
            filePicker.FileTypeFilter.Add(".jfif");
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
                var blobUrl = storageManager.UploadFile(stream, file.FileType);
                return blobUrl;
            }
        }

        private void DeletePicture(string blobUrl)
        {
            var storageManager = new Storage(AppConfiguration.StorageConfig);
            var blobUri = new Uri(blobUrl);
            storageManager.DeleteFile(blobUri);
        }

        private async void Play(SpeechSynthesisResult speech)
        {
            //var stream = new MemoryStream(speech.AudioData);
            //var mPlayer = new MediaPlayer();
            //mPlayer.AudioCategory = MediaPlayerAudioCategory.Speech;
            //mPlayer.Source = MediaSource.CreateFromStream(stream.AsRandomAccessStream(), "audio/wave");
            //mPlayer.Play();

            using (var audioStream = AudioDataStream.FromResult(speech))
            {
                var filePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "outputaudio_for_playback.wav");
                await audioStream.SaveToWaveFileAsync(filePath);
                var mediaPlayer = new MediaPlayer();
                //mediaPlayer.MediaEnded += (sender, args) => {
                //    var file = StorageFile.GetFileFromPathAsync(filePath).GetResults();
                //    file.DeleteAsync();                
                //};
                mediaPlayer.Source = MediaSource.CreateFromStorageFile(await StorageFile.GetFileFromPathAsync(filePath));
                mediaPlayer.Play();
            }
        }
    }
}
