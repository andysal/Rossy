using Microsoft.Extensions.Configuration;
using Rossy.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace Rossy.App
{
    class AppConfig
    {
        private readonly IConfigurationRoot _configurationRoot;
        public Modem.Configuration ModemConfig => GetSection<Modem.Configuration>(nameof(Modem));
        public Rosetta.Configuration RosettaConfig => GetSection<Rosetta.Configuration>(nameof(Rosetta));
        public Sherlock.Configuration SherlockConfig => GetSection<Sherlock.Configuration>(nameof(Sherlock));
        public Storage.Configuration StorageConfig => GetSection<Storage.Configuration>(nameof(Storage));

        public AppConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Package.Current.InstalledLocation.Path)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.production.json", optional: true);

            _configurationRoot = builder.Build();
        }

        private T GetSection<T>(string key) => _configurationRoot.GetSection(key).Get<T>();
    }
}
