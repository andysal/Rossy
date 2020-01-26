using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using Rossy.Analyzers;

namespace Rossy
{
    public class Sherlock
    {
        public Configuration Config { get; private set; }

        public Sherlock(Configuration configuration)
        {
            Config = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public AnalysisResult Analyze(string imageUrl, string intent)
        {
            var analyzer = GetAnalyzer(intent);
            List<VisualFeatureTypes> features = analyzer.SetupAnalysisFeatures();

            var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(Config.SubscriptionKey)) { Endpoint = Config.Endpoint };
            ImageAnalysis imageAnalysis = client.AnalyzeImageAsync(imageUrl, features).Result;
            
            string log = analyzer.ProduceLog(imageAnalysis);
            string speechText = analyzer.ProduceSpeechText(imageAnalysis);

            return new AnalysisResult(speechText, log);
        }

        private IAnalyzer GetAnalyzer(string intent)
        {
            switch (intent)
            {
                case "People":
                    return new PeopleAnalysis();
                case "FullScan":
                default:
                    return new FullScanAnalysis();
            }
        }

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
    }
}
