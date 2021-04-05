using System;
using System.IO;
using System.Threading.Tasks;

namespace Rossy.Runner
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new AppConfig().GetConfig();

            string utterance = "what's up?";
            string filePath = @"C:\Temp\Tricorder\etc\WP_20170520_17_30_04_Rich.jpg";
            using (var fileStream = File.Open(filePath, FileMode.Open))
            {
                var analyzer = new Geordi(config);
                Geordi.AnalysisResult response = await analyzer.AnalyzeAsync(fileStream, utterance);

                var modem = new Modem(config.ModemConfig);
                await modem.ProduceSpeechAsync(response.Result);

                Console.WriteLine(response.Log);
            }
            Console.ReadLine();
        }
    }
}
