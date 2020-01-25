using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rossy
{
    public class Rosetta
    {
        public class Configuration
        {
            public string Endpoint { get; set; }
            public string PredictionKey { get; set; }

            public string AppIdEN { get; set; }
            public string AppIdIT { get; set; }
        }

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

        private LUISRuntimeClient CreateClient()
        {
            var credentials = new ApiKeyServiceClientCredentials(Config.PredictionKey);
            var luisClient = new LUISRuntimeClient(credentials, new System.Net.Http.DelegatingHandler[] { })
            {
                Endpoint = Config.Endpoint
            };

            return luisClient;
        }

        private string GuessLanguage(string utterance)
        {
            return "en-US";
        }

        private string GetAppId(string language)
        {
            switch(language)
            {
                case "it-IT":
                    return Config.AppIdIT;
                case "en-US":
                default:
                    return Config.AppIdEN;
            }
        }

        private PredictionResponse GetPrediction(string utterance)
        {
            var language = GuessLanguage(utterance);
            var appId = GetAppId(language);
            using (var luisClient = CreateClient())
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
    }
}
