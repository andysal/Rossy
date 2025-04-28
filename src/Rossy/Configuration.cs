namespace Rossy
{
    public class Configuration
    {
        public Modem.Configuration ModemConfig { get; set; }
        public Rosetta.Configuration RosettaConfig { get; set; }
        public Geordi.Configuration FaceDetectionServiceConfig { get; set; }
        public Geordi.Configuration ImageAnalysysServiceConfig { get; set; }
    }
}
