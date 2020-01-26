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
                return synthesizer.SpeakTextAsync(story).Result;
            }
        }
        public class Configuration
        {
            public string Endpoint { get; set; }
            public string Key { get; set; }
            public string Region { get; set; }
        }
    }
}
