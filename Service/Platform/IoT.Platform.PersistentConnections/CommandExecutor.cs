using System;
using Thriot.Framework.Logging;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.Platform.Model.Messaging;
using Thriot.Platform.PersistentConnections.Commands;
using Thriot.Platform.Services.Messaging;
using Thriot.Platform.Services.Telemetry;

namespace Thriot.Platform.PersistentConnections
{
    public class CommandExecutor
    {
        private IPersistentConnection _connection;
        private readonly PusherRegistry _pusherRegistry;
        private readonly ConnectionRegistry _connectionRegistry;
        private readonly IMessagingOperations _outgoingMessageReader;
        private readonly IDeviceAuthenticator _deviceAuthenticator;
        private readonly IDeviceOperations _deviceOperations;
        private readonly MessagingService _messagingService;
        private readonly TelemetryDataService _telemetryDataService;
        private static readonly ILogger Logger = LoggerFactory.GetCurrentClassLogger();

        public CommandExecutor(PusherRegistry pusherRegistry, ConnectionRegistry connectionRegistry, IMessagingOperations outgoingMessageReader, IDeviceAuthenticator deviceAuthenticator, IDeviceOperations deviceOperations, MessagingService messagingService, TelemetryDataService telemetryDataService)
        {
            _pusherRegistry = pusherRegistry;
            _connectionRegistry = connectionRegistry;
            _outgoingMessageReader = outgoingMessageReader;
            _deviceAuthenticator = deviceAuthenticator;
            _deviceOperations = deviceOperations;
            _messagingService = messagingService;
            _telemetryDataService = telemetryDataService;
        }

        public void Execute(IPersistentConnection connection, Command command)
        {
            _connection = connection;

            if (command == null || !command.IsValid)
            {
                _connection.Reply("badcommand");
                return;
            }

            if (command is LoginCommand)
            {
                HandleLoginCommand((LoginCommand)command);
                return;
            }

            if (command is SubscribeCommand)
            {
                HandleSubscribeCommand((SubscribeCommand)command);
                return;
            }

            if (command is UnsubscribeCommand)
            {
                HandleUnsubscribeCommand();
                return;
            }

            if (command is CloseCommand)
            {
                HandleCloseCommand();
                return;
            }

            if (command is HeartbeatCommand)
            {
                HandleHeartbeatCommand();
                return;
            }

            if (command is CommitCommand)
            {
                HandleCommitCommand();
                return;
            }

            if (command is TelemetryDataCommand)
            {
                HandleTelemetryDataCommand((TelemetryDataCommand)command);
                return;
            }

            if (command is SendToCommand)
            {
                HandleSendToCommand((SendToCommand)command);
                return;
            }

            _connection.Reply("badcommand");
        }

        private void HandleLoginCommand(LoginCommand loginCommand)
        {
            if (_connection.ConnectionState != ConnectionState.Initiated)
            {
                _connection.Reply("login badcommand");
                return;
            }

            if (_connectionRegistry.IsLoggedIn(loginCommand.DeviceId))
            {
                _connection.Reply("login badcommand");
                return;
            }

            if (
                !_deviceAuthenticator.Authenticate(new AuthenticationParameters(loginCommand.DeviceId,
                    loginCommand.ApiKey)))
            {
                Logger.Error("Login unauthorized. Device: {0}", loginCommand.DeviceId);

                _connection.Reply("login unauthorized");
                return;
            }

            var numericDeviceId = _deviceOperations.Get(loginCommand.DeviceId).NumericId;

            _connectionRegistry.PromoteToLoggedInConnection(_connection, loginCommand.DeviceId, numericDeviceId);
            _connection.Reply("login ack");
        }

        private void HandleSubscribeCommand(SubscribeCommand subscribeCommand)
        {
            if (!_connection.ConnectionState.HasFlag(ConnectionState.LoggedIn))
            {
                _connection.Reply("subscribe unauthorized");
                return;
            }

            if (_connection.ConnectionState.HasFlag(ConnectionState.Subscribed))
            {
                _connection.Reply("subscribe already");
                return;
            }

            if (!Enum.IsDefined(typeof(SubscriptionType), subscribeCommand.SubscriptionType) || subscribeCommand.SubscriptionType == SubscriptionType.None)
            {
                _connection.Reply("subscribe badcommand");
                return;
            }

            _connectionRegistry.PromoteToSubscribedConnection(_connection.DeviceId, subscribeCommand.SubscriptionType);
            _connection.Reply("subscribe ack");
        }

        private void HandleUnsubscribeCommand()
        {
            if (!_connection.ConnectionState.HasFlag(ConnectionState.LoggedIn))
            {
                _connection.Reply("unsubscribe unauthorized");
                return;
            }

            if (!_connection.ConnectionState.HasFlag(ConnectionState.Subscribed))
            {
                _connection.Reply("unsubscribe notsubscribed");
                return;
            }

            _connectionRegistry.UnsubscribeConnection(_connection.DeviceId);
            _connection.Reply("unsubscribe ack");
        }

        private void HandleCloseCommand()
        {
            _connectionRegistry.CloseConnection(_connection);
            _connection.Close();
        }

        private void HandleHeartbeatCommand()
        {
            _connection.Heartbeat();
        }

        private void HandleCommitCommand()
        {
            if (!_connection.ConnectionState.HasFlag(ConnectionState.LoggedIn))
                return;

            if (!_connection.ConnectionState.HasFlag(ConnectionState.Subscribed))
                return;

            if (!_pusherRegistry.IsCommitable(_connection.DeviceId))
                return;

            var state = _outgoingMessageReader.Commit(_connection.NumericDeviceId);

            if (state == OutgoingState.Ok)
            {
                _pusherRegistry.CommitSuccess(_connection.DeviceId);
            }

            _connection.Heartbeat();
        }

        private void HandleTelemetryDataCommand(TelemetryDataCommand command)
        {
            if (!_connection.ConnectionState.HasFlag(ConnectionState.LoggedIn))
            {
                _connection.Reply("telemetrydata unauthorized");
                return;
            }
            
            try
            {
                _telemetryDataService.RecordTelemetryData(_connection.DeviceId, command.TelemetryData);
                _connection.Reply("telemetrydata ack");
            }
            catch (Exception ex)
            {
                Logger.Error("Telemetry record error. Device: {0}. {1}", _connection.DeviceId, ex.ToString());
                _connection.Reply("telemetrydata error");
            }

            _connection.Heartbeat();
        }

        private void HandleSendToCommand(SendToCommand command)
        {
            if (!_connection.ConnectionState.HasFlag(ConnectionState.LoggedIn))
            {
                _connection.Reply("sendto unauthorized");
                return;
            }

            try
            {
                var state = _messagingService.RecordOutgoingMessage(_connection.DeviceId, command.DeviceId, command.Message);
                if(state == OutgoingState.Fail)
                    _connection.Reply("sendto error");
                if (state == OutgoingState.Throttled)
                    _connection.Reply("sendto throttled");

                _connection.Reply("sendto ack");
            }
            catch(Exception ex)
            {
                Logger.Error("Send to device error. Device: {0}. {1}", _connection.DeviceId, ex.ToString());
                _connection.Reply("sendto error");
            }

            _connection.Heartbeat();
        }
    }
}
