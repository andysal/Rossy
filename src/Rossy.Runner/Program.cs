using System;
using System.IO;
using Rossy.IO;

namespace Rossy.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new AppConfig();

            string utterance = "what's up?";
            string filePath = @"C:\Temp\Tricorder\etc\WP_20170520_17_30_04_Rich.jpg";

            using (var fileStream = File.Open(filePath, FileMode.Open))
            {
                var storageManager = new Storage(config.StorageConfig);
                var blobUrl = storageManager.UploadFile(fileStream);
                fileStream.Close();
                fileStream.Dispose();

                var rosetta = new Rosetta(config.RosettaConfig);
                var intent = rosetta.GuessIntent(utterance);
                var analyzer = new Sherlock(config.SherlockConfig);
                Sherlock.AnalysisResult response = analyzer.Analyze(blobUrl, intent);

                var modem = new Modem(config.ModemConfig);
                modem.ProduceSpeech(response.Result);

                Console.WriteLine(response.Log);

                var blobUri = new Uri(blobUrl);
                storageManager.DeleteFile(blobUri);
            }
            Console.ReadLine();
        }
    }
}
