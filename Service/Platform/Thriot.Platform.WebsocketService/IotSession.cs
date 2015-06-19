using System;
using SuperSocket.SocketBase;
using SuperSocket.WebSocket;
using SuperSocket.WebSocket.SubProtocol;
using Thriot.Framework;
using Thriot.Framework.Logging;
using Thriot.Platform.Model.Messaging;
using Thriot.Platform.PersistentConnections;

namespace Thriot.Platform.WebsocketService
{
    class IotSession : WebSocketSession<IotSession>, IPersistentConnection
    {
        private readonly ConnectionRegistry _connectionRegistry;

        private new static readonly ILogger Logger = LoggerFactory.GetCurrentClassLogger();

        public IotSession()
        {
            _connectionRegistry = SingleContainer.Instance.Resolve<ConnectionRegistry>();
        }

        protected override void OnSessionStarted()
        {
            ConnectionId = Guid.NewGuid();
            _connectionRegistry.RegisterInitiatedConnection(this);
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            _connectionRegistry.CloseConnection(this);
        }

        protected override void HandleUnknownCommand(SubRequestInfo requestInfo)
        {
            var remoteEndPoint = this.RemoteEndPoint.Address.ToString();
            Logger.Trace("Executing command: {0}. Device: {1}. IP: {2} ", requestInfo.Key, this.DeviceId, remoteEndPoint);

            try
            {
                var messageString = requestInfo.Key + " " + requestInfo.Body;

                var commandResolver = new CommandResolver(messageString);
                var command = commandResolver.GetCommand();

                var commandExecutor = SingleContainer.Instance.Resolve<CommandExecutor>();
                commandExecutor.Execute(this, command);

                Logger.Trace("Executed command: {0}. Device: {1}. IP: {2}.", requestInfo.Key, this.DeviceId, remoteEndPoint);
            }
            catch (Exception ex)
            {
                Logger.Error("Executed command with Exception: {0}. Device: {1}. IP: {2}. Exception: {3}",
                    requestInfo.Key, this.DeviceId, remoteEndPoint, ex.ToString());
            }
        }

        protected override void HandleException(Exception e)
        {
            Logger.Error("Exception occured. Device: {0}. IP: {1}. Exception: {2}", this.DeviceId, this.RemoteEndPoint.Address.ToString(), e.ToString());
        }

        public void Reply(string response)
        {
            Send(response);
        }

        public void Heartbeat()
        {
            LastHeartbeat = DateTime.UtcNow;
        }

        public void SendMessage(OutgoingMessageToStoreWithState msg)
        {
            if (msg.HasMessage)
            {
                Send(msg.ToStringMessage());
            }
        }

        public TimeSpan HeartbeatValidityPeriod => TimeSpan.FromMinutes(1.5);

        public Guid ConnectionId { get; private set; }
        public string DeviceId { get; set; }
        public long NumericDeviceId { get; set; }
        public ConnectionState ConnectionState { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
        public DateTime LastHeartbeat { get; private set; }
        public DateTime NextReceiveAndForgetTime { get; set; }
        public DateTime NextPeekTime { get; set; }
        public DateTime LastCommitTime { get; set; }
    }
}