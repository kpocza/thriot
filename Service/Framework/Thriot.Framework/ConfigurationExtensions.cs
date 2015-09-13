using Microsoft.Framework.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Thriot.Framework
{
    public static class ConfigurationExtensions
    {
        public static bool HasRootSection(this IConfiguration configuration, string parentKey)
        {
            return configuration.GetChildren().Any(section => section.Key == parentKey);
        }

        public static IDictionary<string, string> AsMap(this IConfiguration configuration, string parentKey)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var section in configuration.GetSection(parentKey).GetChildren())
            {
                var key = section.Key;
                var value = configuration[key];

                dictionary.Add(key.Substring(parentKey.Length + 1), value);
            }

            return dictionary;
        }

        public static IDictionary<Type, Type> AsTypeMap(this IConfiguration configuration, string parentKey)
        {
            return AsMap(configuration, parentKey).ToDictionary(d => Type.GetType(d.Key), d => Type.GetType(d.Value));
        }
    }
}
