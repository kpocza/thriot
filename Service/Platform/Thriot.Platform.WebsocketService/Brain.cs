using System;
using System.Linq;
using SuperSocket.SocketBase.Config;
using Microsoft.Framework.DependencyInjection;
using Thriot.Framework;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.Platform.Model;
using Thriot.Platform.PersistentConnections;
using Thriot.Platform.Services.Messaging;
using Thriot.Platform.Services.Telemetry.Configuration;
using Thriot.Platform.Services.Telemetry.Metadata;
using Thriot.ServiceClient.Messaging;
using Thriot.ServiceClient.TelemetrySetup;

namespace Thriot.Platform.WebsocketService
{
    class Brain
    {
        private readonly IServiceProvider _serviceProvider;
        private PersistentConnectionReceiveAndForgetWorker _persistentConnectionReceiveAndForgetWorker;
        private PersistentConnectionPeekWorker _persistentConnectionPeekWorker;
        private IotWebSocketServer _iotWebSocketServer;
        private ConnectionRegistry _connectionRegistry;

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
            _iotWebSocketServer = new IotWebSocketServer();
            _iotWebSocketServer.Setup(new RootConfig()
            {
                MaxWorkingThreads = 100,
                MaxCompletionPortThreads = 100,
                MinCompletionPortThreads = 20,
                MinWorkingThreads = 20,
            }, new ServerConfig
            {
                Port = 8080,
                MaxConnectionNumber = 10000,
                SendingQueueSize = 500,
                ListenBacklog = 500,
                MaxRequestLength = 2048,
                ReceiveBufferSize = 10240,
                SendBufferSize = 10240,
                SyncSend = false,
            });
            _iotWebSocketServer.SetConnectionRegistry(_connectionRegistry, () => _serviceProvider.GetService<CommandExecutor>());
            _iotWebSocketServer.Start();
        }

        private void StopWebSocketServer()
        {
            _iotWebSocketServer.Stop();
        }

        private void StartBackgrounProcesses()
        {
            var settingOperations = _serviceProvider.GetService<ISettingOperations>();

            SetupTelemetryDataSinkMetadataRegistry(settingOperations);

            var messagingService = _serviceProvider.GetService<IMessagingService>();
            messagingService.Setup(settingOperations.Get(Setting.MessagingServiceEndpoint).Value, settingOperations.Get(Setting.MessagingServiceApiKey).Value);

            var batchParameters = _serviceProvider.GetService<IBatchParameters>();
            MessagingWorkers.Start(batchParameters, messagingService);

            _persistentConnectionReceiveAndForgetWorker =_serviceProvider.GetService<PersistentConnectionReceiveAndForgetWorker>();
            _persistentConnectionReceiveAndForgetWorker.Start();

            _persistentConnectionPeekWorker = _serviceProvider.GetService<PersistentConnectionPeekWorker>();
            _persistentConnectionPeekWorker.Start();

            _connectionRegistry = _serviceProvider.GetService<ConnectionRegistry>();
            _connectionRegistry.Start();
        }

        private void SetupTelemetryDataSinkMetadataRegistry(ISettingOperations settingOperations)
        {
            var telemetryDataSinkSetupService = _serviceProvider.GetService<ITelemetryDataSinkSetupService>();

            telemetryDataSinkSetupService.Setup(settingOperations.Get(Setting.TelemetrySetupServiceEndpoint).Value,
                settingOperations.Get(Setting.TelemetrySetupServiceApiKey).Value);

            var telemetryDataSinkMetadataRegistry = (TelemetryDataSinkMetadataRegistry)_serviceProvider.GetService<ITelemetryDataSinkMetadataRegistry>();
            var telemetryDataSinksMetadata = telemetryDataSinkSetupService.GetTelemetryDataSinksMetadata();

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
