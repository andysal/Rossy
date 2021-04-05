﻿using Microsoft.Extensions.Configuration;
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
        
        public AppConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Package.Current.InstalledLocation.Path)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.production.json", optional: true);

            _configurationRoot = builder.Build();
        }

        private T GetSection<T>(string key) => _configurationRoot.GetSection(key).Get<T>();

        public Configuration GetConfig()
        {
            var config = new Configuration()
            {
                ModemConfig = GetSection<Modem.Configuration>(nameof(Modem)),
                RosettaConfig = GetSection<Rosetta.Configuration>(nameof(Rosetta)),
                GeordiConfig = GetSection<Geordi.Configuration>(nameof(Geordi))
            };
            return config;
        }
    }
}
