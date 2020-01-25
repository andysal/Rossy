using Rossy.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Rossy.App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void btnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            var config = new AppConfig();

            string utterance = txtUtterance.Text;
            string filePath = @"C:\Temp\Tricorder\etc\WP_20170520_17_30_04_Rich.jpg";

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
    }
}
