using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            var credentials = new ApiKeyServiceClientCredentials(Config.TextAnalysisSubscriptionKey);
            
            var client = new TextAnalyticsClient(credentials)
            {
                Endpoint = Config.TextAnalysisEndpoint
            };
            var result = client.DetectLanguage(utterance, "");
            if(result.DetectedLanguages==null || result.DetectedLanguages.Count == 0)
                return "en";
            else
                return result.DetectedLanguages[0].Iso6391Name;
        }

        private string GetAppId(string language)
        {
            switch(language)
            {
                case "it":
                    return Config.AppIdIT;
                case "en":
                default:
                    return Config.AppIdEN;
            }
        }

        private PredictionResponse GetPrediction(string utterance)
        {
            var language = GuessLanguage(utterance);
            var appId = GetAppId(language);
            var credentials = new ApiKeyServiceClientCredentials(Config.PredictionKey);
            using (var luisClient = new LUISRuntimeClient(credentials, new System.Net.Http.DelegatingHandler[] { })
                                            {
                                                Endpoint = Config.Endpoint
                                            })
            {
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
