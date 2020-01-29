using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using Rossy.Analyzers;

namespace Rossy
{
    public class Sherlock
    {
        public Rossy.Configuration RossyConfig { get; private set; }

        public Sherlock(Rossy.Configuration rossyConfiguration)
        {
            RossyConfig = rossyConfiguration ?? throw new ArgumentNullException(nameof(rossyConfiguration));
        }

        public AnalysisResult Analyze(string imageUrl, string utterance)
        {
            var rosetta = new Rosetta(RossyConfig.RosettaConfig);
            var intent = rosetta.GuessIntent(utterance);
            var analyzer = GetAnalyzer(intent);
            List<VisualFeatureTypes> features = analyzer.SetupAnalysisFeatures();

            var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(RossyConfig.SherlockConfig.SubscriptionKey)) { Endpoint = RossyConfig.SherlockConfig.Endpoint };
            ImageAnalysis imageAnalysis = client.AnalyzeImageAsync(imageUrl, features).Result;
            
            string log = analyzer.ProduceLog(imageAnalysis);
            var language = rosetta.GuessLanguage(utterance);
            string speechText;
            switch (language)
            {
                case "it":
                    speechText = analyzer.ProduceSpeechTextItalian(imageAnalysis);
                    break;
                case "en":
                default:
                    speechText = analyzer.ProduceSpeechTextEnglish(imageAnalysis);
                    break;
            }

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
