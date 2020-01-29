using Rossy.IO;

namespace Rossy
{
    public class Configuration
    {
        public Modem.Configuration ModemConfig { get; set; }
        public Rosetta.Configuration RosettaConfig { get; set; }
        public Sherlock.Configuration SherlockConfig { get; set; }
        public Storage.Configuration StorageConfig { get; set; }
    }
}
