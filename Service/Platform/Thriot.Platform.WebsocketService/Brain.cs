using System.Configuration;
using SuperSocket.SocketBase.Config;
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
        private PersistentConnectionReceiveAndForgetWorker _persistentConnectionReceiveAndForgetWorker;
        private PersistentConnectionPeekWorker _persistentConnectionPeekWorker;
        private IotWebSocketServer _iotWebSocketServer;
        private ConnectionRegistry _connectionRegistry;

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
            _iotWebSocketServer.SetConnectionRegistry(_connectionRegistry, () => SingleContainer.Instance.Resolve<CommandExecutor>());
            _iotWebSocketServer.Start();
        }

        private void StopWebSocketServer()
        {
            _iotWebSocketServer.Stop();
        }

        private void StartBackgrounProcesses()
        {
            var settingOperations = SingleContainer.Instance.Resolve<ISettingOperations>();

            SetupTelemetryDataSinkMetadataRegistry(settingOperations);

            var messagingService = SingleContainer.Instance.Resolve<IMessagingService>();
            messagingService.Setup(settingOperations.Get(Setting.MessagingServiceEndpoint).Value, settingOperations.Get(Setting.MessagingServiceApiKey).Value);

            var batchParameters = SingleContainer.Instance.Resolve<IBatchParameters>();
            MessagingWorkers.Start(batchParameters, messagingService);

            _persistentConnectionReceiveAndForgetWorker =
                SingleContainer.Instance.Resolve<PersistentConnectionReceiveAndForgetWorker>();
            _persistentConnectionReceiveAndForgetWorker.Start();

            _persistentConnectionPeekWorker = SingleContainer.Instance.Resolve<PersistentConnectionPeekWorker>();
            _persistentConnectionPeekWorker.Start();

            _connectionRegistry = SingleContainer.Instance.Resolve<ConnectionRegistry>();
            _connectionRegistry.Start();
        }

        private static void SetupTelemetryDataSinkMetadataRegistry(ISettingOperations settingOperations)
        {
            var telemetryDataSinkSetupService = SingleContainer.Instance.Resolve<ITelemetryDataSinkSetupService>();

            telemetryDataSinkSetupService.Setup(settingOperations.Get(Setting.TelemetrySetupServiceEndpoint).Value,
                settingOperations.Get(Setting.TelemetrySetupServiceApiKey).Value);

            var telemetryDataSinkMetadataRegistry =
                (TelemetryDataSinkMetadataRegistry) SingleContainer.Instance.Resolve<ITelemetryDataSinkMetadataRegistry>();
            var telemetryDataSinksMetadata = telemetryDataSinkSetupService.GetTelemetryDataSinksMetadata();

            var telemeryDataSection = new TelemetryDataSection {Incoming = new TelemetryDataSinkCollection()};
            foreach (var inc in telemetryDataSinksMetadata.Incoming)
            {
                var telemetryDataSinkElement = new TelemetryDataSinkElement
                {
                    Name = inc.Name,
                    Type = inc.TypeName,
                    Description = inc.Description,
                    ParameterPresets = new KeyValueConfigurationCollection()
                };
                foreach (var pp in inc.ParametersPresets)
                {
                    telemetryDataSinkElement.ParameterPresets.Add(pp.Key, pp.Value);
                }
                telemeryDataSection.Incoming.Add(telemetryDataSinkElement);
            }
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
