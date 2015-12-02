using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Thriot.Framework;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.Platform.Services.Client;
using Thriot.Platform.Services.Telemetry;
using Thriot.Platform.Services.Telemetry.Configuration;
using Thriot.Platform.Services.Telemetry.Metadata;
using Thriot.Platform.Services.Telemetry.Recording;
using Thriot.Plugins.Core;

namespace Thriot.Platform.TelemetryQueueService
{
    public class ServicesSetup
    {
        private readonly IServiceCollection _services;
        private IServiceProvider _serviceProvider;

        public ServicesSetup()
        {
            _services = new ServiceCollection();
        }

        public void Setup()
        {
            var appFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Framework.Logging.NLogLogger.SetConfiguration(
                Path.Combine(Path.Combine(appFolder, "config"), "nlog.config"));

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.SetBasePath(appFolder);
            configurationBuilder.AddJsonFile("config/services.json");
            configurationBuilder.AddJsonFile("config/connectionstring.json");

            configurationBuilder.AddJsonFile("config/telemetryqueue.json");

            var configuration = configurationBuilder.Build();

            ConfigureThriotServices(configuration);

            _serviceProvider = _services.BuildServiceProvider();

            SetupTelemetryDataSinkMetadataRegistry();
        }

        public IServiceProvider GetServiceProvider()
        {
            return _serviceProvider;
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
            _services.AddSingleton<ITelemetryDataSinkSetupServiceClient, TelemetryDataSinkSetupServiceClient>();
            _services.AddSingleton<Framework.DataAccess.IConnectionParametersResolver, Framework.DataAccess.ConnectionParametersResolver>();
            _services.AddSingleton(_ => configuration);
            _services.AddTransient<IDirectTelemetryDataService, DirectTelemetryDataService>();
            _services.AddTransient<QueueProcessor>();

            var telemetryQueueConfiguration = configuration.AsMap("TelemetryQueue");
            var queueReceiveAdapterType = Type.GetType(telemetryQueueConfiguration["QueueReceiveAdapter"]);

            _services.AddTransient<IQueueReceiveAdapter>(_ =>
            {
                var queueReceiveAdapter = (IQueueReceiveAdapter)Activator.CreateInstance(queueReceiveAdapterType);
                queueReceiveAdapter.Setup(telemetryQueueConfiguration);
                return queueReceiveAdapter;
            });


            foreach (var extraService in configuration.AsTypeMap("Services"))
            {
                _services.AddTransient(extraService.Key, extraService.Value);
            }
        }


        private void SetupTelemetryDataSinkMetadataRegistry()
        {
            var telemetryDataSinkSetupServiceClient = _serviceProvider.GetService<ITelemetryDataSinkSetupServiceClient>();
            var settingOperations = _serviceProvider.GetService<ISettingOperations>();

            telemetryDataSinkSetupServiceClient.Setup(settingOperations.Get(Setting.TelemetrySetupServiceEndpoint).Value,
                settingOperations.Get(Setting.TelemetrySetupServiceApiKey).Value);

            var telemetryDataSinkMetadataRegistry = (TelemetryDataSinkMetadataRegistry)_serviceProvider.GetService<ITelemetryDataSinkMetadataRegistry>();
            var telemetryDataSinksMetadata = telemetryDataSinkSetupServiceClient.GetTelemetryDataSinksMetadata();

            var telemeryDataSection = new TelemetryDataSection
            {
                Incoming = telemetryDataSinksMetadata.Incoming.Select(inc =>
                new TelemetryDataSinkElement
                {
                    Name = inc.Name,
                    Type = inc.TypeName,
                    Description = inc.Description,
                    ParameterPresets = inc.ParametersPresets?.Select(pp =>
                        new TelemetrySinkParameter
                        {
                            Key = pp.Key,
                            Value = pp.Value
                        }).ToArray()
                }).ToArray()
            };

            telemetryDataSinkMetadataRegistry.Build(telemeryDataSection);
        }
    }
}
