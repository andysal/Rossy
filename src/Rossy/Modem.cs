using Kevsoft.Ssml;
using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rossy
{
    public class Modem
    {
        public Configuration Config { get; private set; }

        public Modem(Configuration configuration)
        {
            Config = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<SpeechSynthesisResult> ProduceSpeechAsync(string story)
        {
            var config = SpeechConfig.FromSubscription(Config.Key, Config.Region);
            using var synthesizer = new SpeechSynthesizer(config);
            return await synthesizer.SpeakSsmlAsync(story);
        }

        public (ResultReason, string) Listen()
        {
            var sourceLanguageConfigs = new SourceLanguageConfig[]
            {
                            SourceLanguageConfig.FromLanguage("en-US"),
                            SourceLanguageConfig.FromLanguage("it-IT")
            };
            var config = SpeechTranslationConfig.FromSubscription(Config.Key, Config.Region);
            var autoDetectSourceLanguageConfig = AutoDetectSourceLanguageConfig.FromSourceLanguageConfigs(sourceLanguageConfigs);

            using var recognizer = new SpeechRecognizer(config, autoDetectSourceLanguageConfig);
            var result = recognizer.RecognizeOnceAsync().Result;
            return result.Reason switch
            {
                ResultReason.RecognizingSpeech => (ResultReason.RecognizingSpeech, result.Text),
                _ => (ResultReason.NoMatch, null)
            };
        }

        public class Configuration
        {
            public string Endpoint { get; set; }
            public string Key { get; set; }
            public string Region { get; set; }
        }

        public static async Task<string> BuildSsmlAsync(string text, string language)
        {
            var voice = language switch
            {
                "en" => "en-US-GuyNeural",
                "it" => "it-IT-ElsaNeural",
                _ => "en-US-GuyNeural"
            };
            var ssml = await new Ssml()
                .Say(text)
                .AsVoice(voice)
                .ToStringAsync();
            return ssml;
        }
    }
}
