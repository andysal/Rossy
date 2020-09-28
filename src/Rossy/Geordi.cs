using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using Rossy.Analyzers;
using System.Threading.Tasks;

namespace Rossy
{
    public class Geordi
    {
        public Rossy.Configuration RossyConfig { get; private set; }

        public Geordi(Rossy.Configuration rossyConfiguration)
        {
            RossyConfig = rossyConfiguration ?? throw new ArgumentNullException(nameof(rossyConfiguration));
        }

        public async Task<AnalysisResult> AnalyzeAsync(string imageUrl, string utterance)
        {
            var rosetta = new Rosetta(RossyConfig.RosettaConfig);
            var intent = rosetta.GuessIntent(utterance);
            var analyzer = GetAnalyzer(intent);
            List<VisualFeatureTypes> features = analyzer.SetupAnalysisFeatures();

            var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(RossyConfig.GeordiConfig.SubscriptionKey)) { Endpoint = RossyConfig.GeordiConfig.Endpoint };
            ImageAnalysis imageAnalysis = await client.AnalyzeImageAsync(imageUrl, features);
            
            string log = analyzer.ProduceLog(imageAnalysis);
            var language = rosetta.GuessLanguage(utterance);
            string speechText = language switch
            {
                "it" => analyzer.ProduceSpeechTextItalian(imageAnalysis),
                "en" => analyzer.ProduceSpeechTextEnglish(imageAnalysis),
                _ => analyzer.ProduceSpeechTextEnglish(imageAnalysis)
            };
            return new AnalysisResult(speechText, log);
        }

        private IAnalyzer GetAnalyzer(string intent)
        {
            return intent switch
            {
                "People" => new PeopleAnalysis(),
                _ => new FullScanAnalysis(),
            };
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
