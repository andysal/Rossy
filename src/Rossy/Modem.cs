using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rossy
{
    public class Modem
    {
        public class Configuration
        {
            public string Endpoint { get; set; }
            public string Key { get; set; }
            public string Region { get; set; }
        }

        public Configuration Config { get; private set; }

        public Modem(Configuration configuration)
        {
            Config = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public SpeechSynthesisResult Tell(string story)
        {
            var config = SpeechConfig.FromSubscription(Config.Key, Config.Region);
            using (var synthesizer = new SpeechSynthesizer(config))
            {
                return synthesizer.SpeakTextAsync(story).Result;
            }
        }
    }
}
