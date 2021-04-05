using System;
using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;

namespace Rossy
{
    public class Rosetta
    {
        public Configuration Config { get; private set; }

        public Rosetta(Configuration configuration)
        {
            Config = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string GuessIntent(string utterance)
        {
            var prediction = GetPrediction(utterance);
            return prediction.Prediction.TopIntent;
        }

        public string GuessLanguage(string utterance)
        {
            var endpoint = new Uri(Config.TextAnalysisEndpoint);
            var credentials = new AzureKeyCredential(Config.TextAnalysisSubscriptionKey);
            
            var client = new TextAnalyticsClient(endpoint, credentials);
            DetectedLanguage result = client.DetectLanguage(utterance, "");
            if(string.IsNullOrWhiteSpace(result.Name))
                return "en";
            else
                return result.Iso6391Name;
        }

        private string GetAppId(string language)
        {
            return language switch
            {
                "it" => Config.AppIdIT,
                _ => Config.AppIdEN,
            };
        }

        private PredictionResponse GetPrediction(string utterance)
        {
            var language = GuessLanguage(utterance);
            var appId = GetAppId(language);
            var credentials = new ApiKeyServiceClientCredentials(Config.PredictionKey);
            using var luisClient = new LUISRuntimeClient(credentials, new System.Net.Http.DelegatingHandler[] { })
            {
                Endpoint = Config.Endpoint
            };
            var requestOptions = new PredictionRequestOptions
            {
                DatetimeReference = DateTime.Now, //DateTime.Parse("2019-01-01"),
                PreferExternalEntities = true
            };

            var predictionRequest = new PredictionRequest
            {
                Query = utterance,
                Options = requestOptions
            };

            // get prediction
            return luisClient.Prediction.GetSlotPredictionAsync(
                Guid.Parse(appId),
                slotName: "staging",
                predictionRequest,
                verbose: true,
                showAllIntents: true,
                log: true).Result;
        }

        public class Configuration
        {
            public string TextAnalysisEndpoint { get; set; }
            public string TextAnalysisSubscriptionKey { get; set; }
            public string Endpoint { get; set; }
            public string PredictionKey { get; set; }
            public string AppIdEN { get; set; }
            public string AppIdIT { get; set; }
        }
    }
}
