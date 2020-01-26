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
        }

        public Configuration Config { get; private set; }

        public Modem(Configuration configuration)
        {
            Config = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void Tell(string story)
        {

        }
    }
}
