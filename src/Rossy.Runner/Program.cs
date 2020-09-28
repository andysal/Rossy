using System;
using System.IO;
using System.Threading.Tasks;
using Rossy.IO;

namespace Rossy.Runner
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new AppConfig().GetConfig();

            string utterance = "what's up?";
            string filePath = @"C:\Temp\Tricorder\etc\WP_20170520_17_30_04_Rich.jpg";
            var extension = Path.GetExtension(filePath);
            using (var fileStream = File.Open(filePath, FileMode.Open))
            {
                var storageManager = new Storage(config.StorageConfig);
                var blobUrl = storageManager.UploadFile(fileStream, extension);
                fileStream.Close();
                fileStream.Dispose();

                var analyzer = new Geordi(config);
                Geordi.AnalysisResult response = await analyzer.AnalyzeAsync(blobUrl, utterance);

                var modem = new Modem(config.ModemConfig);
                await modem.ProduceSpeechAsync(response.Result);

                Console.WriteLine(response.Log);

                var blobUri = new Uri(blobUrl);
                storageManager.DeleteFile(blobUri);
            }
            Console.ReadLine();
        }
    }
}
