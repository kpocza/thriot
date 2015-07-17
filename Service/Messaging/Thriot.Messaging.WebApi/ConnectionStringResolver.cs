﻿using Microsoft.Framework.Configuration;
using System.Configuration;
using Thriot.Messaging.Services.Storage;

namespace Thriot.Messaging.WebApi
{
    public class ConnectionStringResolver : IConnectionStringResolver
    {
        private readonly IConfiguration _configuration;

        public ConnectionStringResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ConnectionString
        {
            get { return _configuration.Get($"ConnectionString:MessagingConnection"); }
        }
    }
}