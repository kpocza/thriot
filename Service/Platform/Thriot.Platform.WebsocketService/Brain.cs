using System;
using System.IO;
using System.Linq;
using System.Reflection;
using SuperSocket.SocketBase.Config;
using Microsoft.Extensions.DependencyInjection;
using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.Platform.Model;
using Thriot.Platform.PersistentConnections;
using Thriot.Platform.Services.Messaging;
using Thriot.Platform.Services.Telemetry.Configuration;
using Thriot.Platform.Services.Telemetry.Metadata;
using Thriot.Messaging.Services.Client;
using Thriot.Platform.Services.Client;

namespace Thriot.Platform.WebsocketService
{
    class Brain
    {
        private readonly IServiceProvider _serviceProvider;
        private PersistentConnectionReceiveAndForgetWorker _persistentConnectionReceiveAndForgetWorker;
        private PersistentConnectionPeekWorker _persistentConnectionPeekWorker;
        private ConnectionRegistry _connectionRegistry;
        private IBootstrap _bootstrapFactory;

        public Brain(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        internal void Start()
        {
            StartBackgrounProcesses();
            StartWebSocketServer();
        }

        internal void Stop()
        {
            StopWebSocketServer();
            StopBackgroundProcesses();
        }


        private void StartWebSocketServer()
        {
            var appFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var configPath = Path.Combine(Path.Combine(appFolder, "config"), "supersocket.config");

            _bootstrapFactory = BootstrapFactory.CreateBootstrapFromConfigFile(configPath);
            if(!_bootstrapFactory.Initialize())
                throw new Exception("Failed to initialize");
            (_bootstrapFactory.AppServers.First() as IotWebSocketServer).SetConnectionRegistry(_connectionRegistry, () => _serviceProvider.GetService<CommandExecutor>());

            var result = _bootstrapFactory.Start();

            if(result == StartResult.Failed)
                throw new Exception("Failed to start");
        }

        private void StopWebSocketServer()
        {
            _bootstrapFactory.Stop();
        }

        private void StartBackgrounProcesses()
        {
            var settingOperations = _serviceProvider.GetService<ISettingOperations>();

            SetupTelemetryDataSinkMetadataRegistry(settingOperations);

            var messagingServiceClient = _serviceProvider.GetService<IMessagingServiceClient>();
            messagingServiceClient.Setup(settingOperations.Get(Setting.MessagingServiceEndpoint).Value, settingOperations.Get(Setting.MessagingServiceApiKey).Value);

            var batchParameters = _serviceProvider.GetService<IBatchParameters>();
            MessagingWorkers.Start(batchParameters, messagingServiceClient);

            _persistentConnectionReceiveAndForgetWorker =_serviceProvider.GetService<PersistentConnectionReceiveAndForgetWorker>();
            _persistentConnectionReceiveAndForgetWorker.Start();

            _persistentConnectionPeekWorker = _serviceProvider.GetService<PersistentConnectionPeekWorker>();
            _persistentConnectionPeekWorker.Start();

            _connectionRegistry = _serviceProvider.GetService<ConnectionRegistry>();
            _connectionRegistry.Start();
        }

        private void SetupTelemetryDataSinkMetadataRegistry(ISettingOperations settingOperations)
        {
            var telemetryDataSinkSetupServiceClient = _serviceProvider.GetService<ITelemetryDataSinkSetupServiceClient>();

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

        private void StopBackgroundProcesses()
        {
            MessagingWorkers.Stop();

            _persistentConnectionReceiveAndForgetWorker.Stop();
            _persistentConnectionPeekWorker.Stop();
            _connectionRegistry.Stop();
        }
    }
}
