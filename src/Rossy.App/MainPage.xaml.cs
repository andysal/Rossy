using Microsoft.CognitiveServices.Speech;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Rossy.App
{
    public sealed partial class MainPage : Page
    {
        private Configuration AppConfiguration { get; set; }
        private StorageFile picture = null;

        public MainPage()
        {
            this.InitializeComponent();
            AppConfiguration = new AppConfig().GetConfig();

            txtUtterance.Text = "what's up?";
        }

        private async void btnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            var utterance = string.IsNullOrWhiteSpace(txtUtterance.Text) ? "what's up?" : txtUtterance.Text;

            var analyzer = new Geordi(AppConfiguration);
            var stream = await picture.OpenAsync(FileAccessMode.Read);
            Geordi.AnalysisResult response = await analyzer.AnalyzeAsync(stream.AsStream(), utterance);          

            var modem = new Modem(AppConfiguration.ModemConfig);
            var result = await modem.ProduceSpeechAsync(response.Result);
            Play(result);

            txtAnalysisResult.Text = response.Log;
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

            picture = await filePicker.PickSingleFileAsync();
            imgPhoto.Source = await GetBitmapFromStorageFile(picture);
        }

        private async void btnTakePicture_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CameraCaptureUI();
            picture = await dialog.CaptureFileAsync(CameraCaptureUIMode.Photo);
            imgPhoto.Source = await GetBitmapFromStorageFile(picture);
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

        private static async Task<ImageSource> GetBitmapFromStorageFile(StorageFile sf)
        {
            using var randomAccessStream = await sf.OpenAsync(FileAccessMode.Read);
            var result = new BitmapImage();
            await result.SetSourceAsync(randomAccessStream);
            return result;
        }
    }
}
