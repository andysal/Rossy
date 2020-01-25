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
                var blobUri = new Uri(blobUrl);
                storageManager.DeleteFile(blobUri);
            }
            Console.ReadLine();
        }
    }
}
