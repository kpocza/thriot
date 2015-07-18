﻿using Microsoft.Framework.Configuration;
using System;
using System.Collections.Generic;

namespace Thriot.Framework.Mvc
{
    public static class ServicesResolver
    {
        public static IDictionary<Type, Type> Resolve(IConfiguration configuration, string servicesKey)
        {
            var dictionary = new Dictionary<Type, Type>();
            foreach (var extraService in configuration.GetConfigurationSection(servicesKey).GetConfigurationSections())
            {
                var intf = extraService.Key;
                var impl = configuration.Get($"{servicesKey}:{intf}");

                dictionary.Add(System.Type.GetType(intf), System.Type.GetType(impl));
            }

            return dictionary;
        }
    }
}
