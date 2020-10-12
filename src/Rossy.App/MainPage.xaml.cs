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

namespace Rossy.App
{
    public sealed partial class MainPage : Page
    {
        private Configuration AppConfiguration { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            AppConfiguration = new AppConfig().GetConfig();

            txtFilePath.Text = "";
            txtUtterance.Text = "what's up?";
        }

        private async void btnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            var modem = new Modem(AppConfiguration.ModemConfig);
            if (string.IsNullOrWhiteSpace(txtFilePath.Text))
            {
                var ssml = await Modem.BuildSsmlAsync("Please, either upload or take a new picture.", "en");
                var speech = await modem.ProduceSpeechAsync(ssml);
                Play(speech);
            }
            if (string.IsNullOrWhiteSpace(txtUtterance.Text))
                txtUtterance.Text = "what's up?";

            string blobUrl = txtFilePath.Text;
            var utterance = txtUtterance.Text;

            var bitmapImage = new BitmapImage(new Uri(blobUrl, UriKind.Absolute));
            imgPhoto.Source = bitmapImage;

            var analyzer = new Geordi(AppConfiguration);
            Geordi.AnalysisResult response = await analyzer.AnalyzeAsync(blobUrl, utterance);          

            var result = await modem.ProduceSpeechAsync(response.Result);
            Play(result);

            txtAnalysisResult.Text = response.Log;
            DeletePicture(blobUrl);

            txtFilePath.Text = "";
        }

        private async void btnListen_Click(object sender, RoutedEventArgs e)
        {            
            bool permissionGained = await AudioCapturePermissions.RequestMicrophonePermission();
            if(!permissionGained)
            {
                var ssml = await Modem.BuildSsmlAsync("Could not enable the microphone, please try again.", "en");
                var modem = new Modem(AppConfiguration.ModemConfig);
                var speech = await modem.ProduceSpeechAsync(ssml);
                Play(speech);
            }
            else
            {
                txtUtterance.Text = "(listening...)";
                var modem = new Modem(AppConfiguration.ModemConfig);
                var utterance = await modem.ListenAsync();
                switch(utterance.Item1)
                {
                    case ResultReason.RecognizedSpeech:
                        txtUtterance.Text = utterance.Item2;
                        break;
                    case ResultReason.NoMatch:
                    default:
                        txtUtterance.Text = "";
                        var ssml = await Modem.BuildSsmlAsync("Could not understand utterance, please try again.", "en");
                        var speech = await modem.ProduceSpeechAsync(ssml);
                        Play(speech);
                        break;
                }
            }
        }

        private async void btnPickFile_Click(object sender, RoutedEventArgs e)
        {
            var filePicker = new FileOpenPicker();
            filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            filePicker.ViewMode = PickerViewMode.Thumbnail;

            filePicker.FileTypeFilter.Clear();
            filePicker.FileTypeFilter.Add(".jpeg"); 
            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".jfif");
            filePicker.FileTypeFilter.Add(".png");

            StorageFile file = await filePicker.PickSingleFileAsync();
            if (file != null)
            {
                var blobUrl = await UploadPicture(file);
                txtFilePath.Text = blobUrl;
            }
        }

        private async void btnTakePicture_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CameraCaptureUI();
            var file = await dialog.CaptureFileAsync(CameraCaptureUIMode.Photo);
            var blobUrl = await UploadPicture(file);
            txtFilePath.Text = blobUrl;
        }

        private async Task<string> UploadPicture(StorageFile file)
        {
            var randomAccessStream = await file.OpenReadAsync();
            using Stream stream = randomAccessStream.AsStreamForRead();
            var storageManager = new Storage(AppConfiguration.StorageConfig);
            var blobUrl = storageManager.UploadFile(stream, file.FileType);
            return blobUrl;
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

            using var audioStream = AudioDataStream.FromResult(speech);
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
