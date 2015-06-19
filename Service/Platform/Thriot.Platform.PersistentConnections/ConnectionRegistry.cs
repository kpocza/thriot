using System;
using System.Collections.Generic;
using System.Threading;
using Thriot.Framework;
using Thriot.Framework.Logging;

namespace Thriot.Platform.PersistentConnections
{
    public class ConnectionRegistry
    {
        private readonly PusherRegistry _pusherRegistry;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly object _lock;

        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly AutoResetEvent _taskEndWaitEvent;
        private readonly AutoResetEvent _taskStartWaitEvent;
        private static readonly ILogger Logger = LoggerFactory.GetCurrentClassLogger();

        public ConnectionRegistry(PusherRegistry pusherRegistry, IDateTimeProvider dateTimeProvider)
        {
            _pusherRegistry = pusherRegistry;
            _dateTimeProvider = dateTimeProvider;
            InitiatedConnections = new Dictionary<Guid, IPersistentConnection>();
            LoggedInConnections = new Dictionary<string, IPersistentConnection>();
            SubscribedConnections = new Dictionary<string, IPersistentConnection>();
            _lock = new object();
            _cancellationTokenSource = new CancellationTokenSource();
            _taskEndWaitEvent = new AutoResetEvent(false);
            _taskStartWaitEvent = new AutoResetEvent(false);
        }

        public void Start()
        {
            new Thread(() => DeadConnectionKicker(_cancellationTokenSource.Token)).Start();
            _taskStartWaitEvent.WaitOne();
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _taskEndWaitEvent.WaitOne(1000);
        }

        public void RegisterInitiatedConnection(IPersistentConnection connection)
        {
            lock (_lock)
            {
                connection.ConnectionState = ConnectionState.Initiated;
                connection.Heartbeat();
                InitiatedConnections.Add(connection.ConnectionId, connection);
            }
        }

        public void PromoteToLoggedInConnection(IPersistentConnection connection, string deviceId, long numericDeviceId)
        {
            if(deviceId == null)
                throw new ArgumentNullException(nameof(deviceId));

            lock (_lock)
            {
                InitiatedConnections.Remove(connection.ConnectionId);

                connection.DeviceId = deviceId;
                connection.NumericDeviceId = numericDeviceId;
                connection.ConnectionState = ConnectionState.LoggedIn;
                connection.Heartbeat();
                LoggedInConnections.Add(connection.DeviceId, connection);
            }
        }

        public void PromoteToSubscribedConnection(string deviceId, SubscriptionType subscriptionType)
        {
            lock (_lock)
            {
                var loggedInConnection = LoggedInConnections[deviceId];

                loggedInConnection.ConnectionState = loggedInConnection.ConnectionState | ConnectionState.Subscribed;
                loggedInConnection.SubscriptionType = subscriptionType;
                loggedInConnection.Heartbeat();

                SubscribedConnections.Add(deviceId, loggedInConnection);

                _pusherRegistry.AddConnection(loggedInConnection);
            }
        }

        public void UnsubscribeConnection(string deviceId)
        {
            lock (_lock)
            {
                var subscribedConnection = SubscribedConnections[deviceId];

                subscribedConnection.ConnectionState = ConnectionState.LoggedIn;
                subscribedConnection.SubscriptionType = SubscriptionType.None;
                SubscribedConnections.Remove(deviceId);
            }
        }

        public void CloseConnection(IPersistentConnection connection)
        {
            lock (_lock)
            {
                connection.ConnectionState = ConnectionState.None;
                connection.SubscriptionType = SubscriptionType.None;

                if (connection.DeviceId == null)
                {
                    InitiatedConnections.Remove(connection.ConnectionId);
                }
                else
                {
                    LoggedInConnections.Remove(connection.DeviceId);
                    SubscribedConnections.Remove(connection.DeviceId);
                }
            }
        }

        public bool IsLoggedIn(string deviceId)
        {
            lock (_lock)
            {
                return LoggedInConnections.ContainsKey(deviceId);
            }
        }

        private void DeadConnectionKicker(CancellationToken cancellationToken)
        {
            _taskStartWaitEvent.Set();

            while (!cancellationToken.IsCancellationRequested)
            {
                var now = _dateTimeProvider.UtcNow;
                var collectedToKill = CollectTimeoutedConnections(connection => connection.LastHeartbeat + connection.HeartbeatValidityPeriod < now);

                foreach (var collected in collectedToKill)
                {
                    CloseConnection(collected);
                    try
                    {
                        collected.Close();
                    }
                    catch(Exception ex)
                    {
                        Logger.Warning("Close error. Device: {0}. {1}", collected.DeviceId, ex.ToString());
                    }
                }

                Thread.Sleep(100);
            }

            _taskEndWaitEvent.Set();
        }

        private ICollection<IPersistentConnection> CollectTimeoutedConnections(Func<IPersistentConnection, bool> shouldCollect)
        {
            var collecteds = new List<IPersistentConnection>();
            lock (_lock)
            {
                foreach (var initiated in InitiatedConnections)
                {
                    if (shouldCollect(initiated.Value))
                    {
                        collecteds.Add(initiated.Value);
                    }
                }

                foreach (var loggedIn in LoggedInConnections)
                {
                    if (shouldCollect(loggedIn.Value))
                    {
                        collecteds.Add(loggedIn.Value);
                    }
                }
            }
            return collecteds;
        }


        private IDictionary<Guid, IPersistentConnection> InitiatedConnections { get; set; }

        private IDictionary<string, IPersistentConnection> LoggedInConnections { get; set; }

        private IDictionary<string, IPersistentConnection> SubscribedConnections { get; set; }
    }
}
