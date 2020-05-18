using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rossy
{
    public class Modem
    {
        public Configuration Config { get; private set; }

        public Modem(Configuration configuration)
        {
            Config = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public SpeechSynthesisResult ProduceSpeech(string story)
        {
            var config = SpeechConfig.FromSubscription(Config.Key, Config.Region);
            using (var synthesizer = new SpeechSynthesizer(config))
            {
                return synthesizer.SpeakSsmlAsync(story).Result;
            }
        }

        public (string, string) Listen()
        {
            var sourceLanguageConfigs = new SourceLanguageConfig[]
            {
                SourceLanguageConfig.FromLanguage("en-US"),
                SourceLanguageConfig.FromLanguage("it-IT")
            };
            var autoDetectSourceLanguageConfig = AutoDetectSourceLanguageConfig.FromSourceLanguageConfigs(
                                                    sourceLanguageConfigs);

            var config = SpeechTranslationConfig.FromSubscription(Config.Key, Config.Region);
            var recognizer = new SpeechRecognizer(config);
            var result = recognizer.RecognizeOnceAsync().Result;

            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                var autoDetectSourceLanguageResult = AutoDetectSourceLanguageResult.FromResult(result).Language;
                return (result.Text, autoDetectSourceLanguageResult);
            }
            else
                return ("Speech could not be recognized", null);
        }

        public class Configuration
        {
            public string Endpoint { get; set; }
            public string Key { get; set; }
            public string Region { get; set; }
        }


    }
}
