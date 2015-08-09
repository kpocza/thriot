using Microsoft.Framework.Configuration;
using System;
using System.Collections.Generic;

namespace Thriot.Framework
{
    public static class ServicesConfigLoader
    {
        public static IDictionary<Type, Type> Load(IConfiguration configuration, string servicesKey)
        {
            var dictionary = new Dictionary<Type, Type>();
            foreach (var extraService in configuration.GetConfigurationSection(servicesKey).GetConfigurationSections())
            {
                var intf = extraService.Key;
                var impl = configuration.Get($"{servicesKey}:{intf}");

                dictionary.Add(Type.GetType(intf), Type.GetType(impl));
            }

            return dictionary;
        }
    }
}
