using System.Configuration;
using System.Linq;
using IoT.Framework;
using IoT.Objects.Model;
using IoT.Objects.Model.Operations;
using IoT.Platform.Model;
using IoT.Platform.PersistentConnections;
using IoT.Platform.Services.Messaging;
using IoT.Platform.Services.Telemetry.Configuration;
using IoT.Platform.Services.Telemetry.Metadata;
using IoT.ServiceClient.Messaging;
using IoT.ServiceClient.TelemetrySetup;
using SuperSocket.SocketBase.Config;

namespace IoT.Platform.WebsocketService
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

            _iotWebSocketServer.Start();
        }

        private void StopWebSocketServer()
        {
            _iotWebSocketServer.Stop();
        }

        private void StartBackgrounProcesses()
        {
            var telemetryDataSinkSetupService = SingleContainer.Instance.Resolve<ITelemetryDataSinkSetupService>();
            var settingOperations = SingleContainer.Instance.Resolve<ISettingOperations>();

            telemetryDataSinkSetupService.Setup(settingOperations.Get(Setting.TelemetrySetupServiceEndpoint).Value, settingOperations.Get(Setting.TelemetrySetupServiceApiKey).Value);

            var telemetryDataSinkMetadataRegistry =
                (TelemetryDataSinkMetadataRegistry)SingleContainer.Instance.Resolve<ITelemetryDataSinkMetadataRegistry>();
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

        private void StopBackgroundProcesses()
        {
            MessagingWorkers.Stop();

            _persistentConnectionReceiveAndForgetWorker.Stop();
            _persistentConnectionPeekWorker.Stop();
            _connectionRegistry.Stop();
        }

    }
}
