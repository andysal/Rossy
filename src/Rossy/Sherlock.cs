using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Text;
using System.Linq;
using Rossy.Analyzers;

namespace Rossy
{
    public class Sherlock
    {
        public class Configuration
        {
            public string Endpoint { get; set; }
            public string SubscriptionKey { get; set; }
        }

        public class AnalysisResult
        {
            public AnalysisResult(string result, string log)
            {
                Result = result ?? throw new ArgumentNullException(nameof(result));
                Log = log ?? throw new ArgumentNullException(nameof(log));
            }

            public string Result { get; private set; }
            public string Log { get; private set; }
        }

        public ComputerVisionClient Client { get; private set; }

        public Configuration Config { get; private set; }

        public Sherlock(Configuration configuration)
        {
            Config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Client = Authenticate(Config.Endpoint, Config.SubscriptionKey);
        }

        private ComputerVisionClient Authenticate(string endpoint, string key)
        {
            var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
                                { Endpoint = endpoint };
            return client;
        }

        public AnalysisResult Analyze(string imageUrl, string intent)
        {
            var analyzer = Analyzer.GetAnalyzer(intent);
            List<VisualFeatureTypes> features = analyzer.SetupAnalysisFeatures();
            ImageAnalysis imageAnalysis = Client.AnalyzeImageAsync(imageUrl, features).Result;
            string log = analyzer.ProduceLog(imageAnalysis);
            string speechText = analyzer.ProduceSpeechText(imageAnalysis);

            return new AnalysisResult(speechText, log);
        }
    }
}
