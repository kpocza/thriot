using System;
using System.IO;
using System.Reflection;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Thriot.Framework;
using Thriot.Objects.Common;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.Platform.Model;
using Thriot.Platform.Model.Messaging;
using Thriot.Platform.PersistentConnections;
using Thriot.Platform.Services.Messaging;
using Thriot.Platform.Services.Telemetry;
using Thriot.Platform.Services.Telemetry.Metadata;
using Thriot.ServiceClient.TelemetrySetup;

namespace Thriot.Platform.WebsocketService
{
    public class ServicesSetup
    {
        private readonly IServiceCollection _services;

        public ServicesSetup()
        {
            _services = new ServiceCollection();
        }

        public void Setup()
        {
            var configurationBuilder = new ConfigurationBuilder(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            configurationBuilder.AddJsonFile("config/services.json");
            configurationBuilder.AddJsonFile("config/connectionstring.json");

            var configuration = configurationBuilder.Build();

            _services.AddTransient<ICompanyOperations, Objects.Common.CachingOperations.CompanyOperations>();
            _services.AddTransient<IServiceOperations, Objects.Common.CachingOperations.ServiceOperations>();
            _services.AddTransient<INetworkOperations, Objects.Common.CachingOperations.NetworkOperations>();
            _services.AddTransient<IDeviceOperations, Objects.Common.CachingOperations.DeviceOperations>();
            _services.AddTransient<ISettingOperations, Objects.Common.CachingOperations.SettingOperations>();
            _services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            _services.AddSingleton<Framework.DataAccess.IDynamicConnectionStringResolver, DynamicConnectionStringResolver>();
            _services.AddSingleton<ITelemetryDataSinkMetadataRegistry, TelemetryDataSinkMetadataRegistry>();
            _services.AddTransient<ITelemetryDataSinkResolver, TelemetryDataSinkResolver>();
            _services.AddSingleton<IBatchParameters, BatchParameters>();
            _services.AddSingleton<ITelemetryDataSinkSetupServiceClient, ServiceClient.TelemetrySetup.TelemetryDataSinkSetupServiceClient>();
            _services.AddTransient<IMessagingOperations, MessagingOperations>();
            _services.AddSingleton<ConnectionRegistry>();
            _services.AddSingleton<PusherRegistry>();
            _services.AddTransient<CommandExecutor>();
            _services.AddTransient<PersistentConnectionReceiveAndForgetWorker>();
            _services.AddTransient<PersistentConnectionPeekWorker>();
            _services.AddTransient<Platform.Services.Messaging.MessagingService>();
            _services.AddTransient<Platform.Services.Telemetry.TelemetryDataService>();
            _services.AddTransient<IDeviceAuthenticator, DeviceAuthenticator>();
            _services.AddSingleton<Thriot.ServiceClient.Messaging.IMessagingServiceClient, Thriot.ServiceClient.Messaging.MessagingServiceClient>();
            _services.AddSingleton<Framework.DataAccess.IConnectionParametersResolver, Framework.DataAccess.ConnectionParametersResolver>();
            _services.AddSingleton(_ => configuration);

            foreach (var extraService in Framework.ServicesConfigLoader.Load(configuration, "Services"))
            {
                _services.AddTransient(extraService.Key, extraService.Value);
            }
        }

        public IServiceProvider GetServiceProvider()
        {
            return _services.BuildServiceProvider();
        }
    }
}
