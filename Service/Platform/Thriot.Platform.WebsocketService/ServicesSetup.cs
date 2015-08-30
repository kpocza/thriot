using System;
using System.IO;
using System.Reflection;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Thriot.Framework;
using Thriot.Messaging.Services.Client;
using Thriot.Objects.Common;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.Platform.Model;
using Thriot.Platform.Model.Messaging;
using Thriot.Platform.PersistentConnections;
using Thriot.Platform.Services.Messaging;
using Thriot.Platform.Services.Telemetry;
using Thriot.Platform.Services.Telemetry.Metadata;
using Thriot.Platform.Services.Client;
using Thriot.Platform.Services.Telemetry.Recording;
using Thriot.Plugins.Core;

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
            var appFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Framework.Logging.NLogLogger.SetConfiguration(
                Path.Combine(Path.Combine(appFolder, "config"), "nlog.config"));

            var configurationBuilder = new ConfigurationBuilder(appFolder);
            configurationBuilder.AddJsonFile("config/services.json");
            configurationBuilder.AddJsonFile("config/connectionstring.json");

            configurationBuilder.AddJsonFile("config/telemetryqueue.json", true);

            var configuration = configurationBuilder.Build();

            ConfigureThriotServices(configuration);

            ConfigureTelemetryDataService(configuration);
        }

        public IServiceProvider GetServiceProvider()
        {
            return _services.BuildServiceProvider();
        }

        private void ConfigureThriotServices(IConfiguration configuration)
        {
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
            _services.AddSingleton<ITelemetryDataSinkSetupServiceClient, TelemetryDataSinkSetupServiceClient>();
            _services.AddTransient<IMessagingOperations, MessagingOperations>();
            _services.AddSingleton<ConnectionRegistry>();
            _services.AddSingleton<PusherRegistry>();
            _services.AddTransient<CommandExecutor>();
            _services.AddTransient<PersistentConnectionReceiveAndForgetWorker>();
            _services.AddTransient<PersistentConnectionPeekWorker>();
            _services.AddTransient<MessagingService>();
            _services.AddTransient<IDeviceAuthenticator, DeviceAuthenticator>();
            _services.AddSingleton<IMessagingServiceClient, MessagingServiceClient>();
            _services.AddSingleton<Framework.DataAccess.IConnectionParametersResolver, Framework.DataAccess.ConnectionParametersResolver>();
            _services.AddSingleton(_ => configuration);

            foreach (var extraService in ConfigurationAdapter.LoadServiceConfiguration(configuration, "Services"))
            {
                _services.AddTransient(extraService.Key, extraService.Value);
            }
        }

        private void ConfigureTelemetryDataService(IConfiguration configuration)
        {
            if (!ConfigurationAdapter.HasRootSection(configuration, "TelemetryQueue"))
            {
                _services.AddTransient<ITelemetryDataService, DirectTelemetryDataService>();
            }
            else
            {
                _services.AddTransient<ITelemetryDataService, QueueingTelemetryDataService>();

                var telemetryQueueConfiguration = ConfigurationAdapter.AsMap(configuration, "TelemetryQueue");
                var queueSendAdapterType = Type.GetType(telemetryQueueConfiguration["QueueSendAdapter"]);

                _services.AddTransient<IQueueSendAdapter>(_ =>
                {
                    var queueSendAdapter = (IQueueSendAdapter)Activator.CreateInstance(queueSendAdapterType);
                    queueSendAdapter.Setup(telemetryQueueConfiguration);
                    return queueSendAdapter;
                });
            }
        }
    }
}
