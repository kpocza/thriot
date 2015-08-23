using Microsoft.Framework.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Thriot.Framework
{
    public static class ConfigurationAdapter
    {
        public static bool HasRootSection(IConfiguration configuration, string parentKey)
        {
            return configuration.GetConfigurationSections().Any(section => section.Key == parentKey);
        }

        public static IDictionary<string, string> AsMap(IConfiguration configuration, string parentKey)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var section in configuration.GetConfigurationSection(parentKey).GetConfigurationSections())
            {
                var key = section.Key;
                var value = configuration.Get($"{parentKey}:{key}");

                dictionary.Add(key, value);
            }

            return dictionary;
        }

        public static IDictionary<Type, Type> LoadServiceConfiguration(IConfiguration configuration, string servicesKey)
        {
            return AsMap(configuration, servicesKey).ToDictionary(d => Type.GetType(d.Key), d => Type.GetType(d.Value));
        }
    }
}
