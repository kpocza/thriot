using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Thriot.Framework;

namespace Thriot.Platform.PersistentConnections
{
    public class PusherRegistry
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private ConnectionRegistry _connectionRegistry;
        private DateTime _lastCommitRequeueTime;

        public PusherRegistry(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
            _lastCommitRequeueTime = dateTimeProvider.UtcNow;
            ReceiveAndForgetConnections = new ConcurrentQueue<IPersistentConnection>();
            PeekConnections = new ConcurrentQueue<IPersistentConnection>();
            CommitConnections = new ConcurrentDictionary<string, IPersistentConnection>();
        }

        public void RegisterConnectionRegistry(ConnectionRegistry connectionRegistry)
        {
            _connectionRegistry = connectionRegistry;
        }

        public void AddConnection(IPersistentConnection connection)
        {
            if (connection.SubscriptionType == SubscriptionType.ReceiveAndForget)
            {
                connection.NextReceiveAndForgetTime = ReceiveAndForgetTime;
                ReceiveAndForgetConnections.Enqueue(connection);
                return;
            }
            if (connection.SubscriptionType == SubscriptionType.PeekAndCommit)
            {
                connection.NextPeekTime = PeekTime;
                PeekConnections.Enqueue(connection);
                return;
            }

            throw new NotImplementedException();
        }

        public IPersistentConnection GetReceiveAndForgetConnection()
        {
            while (true)
            {
                IPersistentConnection connection;

                if (!ReceiveAndForgetConnections.TryPeek(out connection))
                {
                    Thread.Sleep(1);
                    return null;
                }

                if (!connection.ConnectionState.HasFlag(ConnectionState.Subscribed) ||
                    connection.SubscriptionType != SubscriptionType.ReceiveAndForget)
                {
                    ReceiveAndForgetConnections.TryDequeue(out connection);
                    continue;
                }

                if (connection.NextReceiveAndForgetTime < _dateTimeProvider.UtcNow) // device can be checked for new outgoing messages
                {
                    if (!ReceiveAndForgetConnections.TryDequeue(out connection))
                        continue;
                    connection.NextReceiveAndForgetTime = ReceiveAndForgetTime;
                    ReceiveAndForgetConnections.Enqueue(connection);
                    return connection;
                }

                Thread.Sleep(1);

                return null; // we are too early
            }
        }

        public IPersistentConnection GetPeekConnection()
        {
            RequeueTimedOutCommits();

            while (true)
            {
                IPersistentConnection connection;

                if (!PeekConnections.TryPeek(out connection))
                {
                    Thread.Sleep(1);
                    return null;
                }

                if (!connection.ConnectionState.HasFlag(ConnectionState.Subscribed) ||
                    connection.SubscriptionType != SubscriptionType.PeekAndCommit)
                {
                    PeekConnections.TryDequeue(out connection);
                    continue;
                }

                if (connection.NextPeekTime < _dateTimeProvider.UtcNow) // device can be checked for new outgoing messages
                {
                    if (!PeekConnections.TryDequeue(out connection))
                        continue;
                    return connection;
                }

                Thread.Sleep(1);

                return null; // we are too early
            }
        }

        private void RequeueTimedOutCommits()
        {
            if (_lastCommitRequeueTime.AddMilliseconds(100) > _dateTimeProvider.UtcNow)
                return;

            _lastCommitRequeueTime = _dateTimeProvider.UtcNow;

            var toRequeue = new List<string>();

            foreach (var commit in CommitConnections)
            {
                if (commit.Value.LastCommitTime + PutBackToPeekInteval < _dateTimeProvider.UtcNow)
                {
                    toRequeue.Add(commit.Key);
                }
            }

            foreach (var toReq in toRequeue)
            {
                CommitSuccess(toReq);
            }
        }

        public void RequeueAsPeekConnections(IEnumerable<IPersistentConnection> connections)
        {
            foreach (var connection in connections)
            {
                connection.NextPeekTime = PeekTime;
                PeekConnections.Enqueue(connection);
            }
        }

        public void SetAsCommitNeededConnections(IEnumerable<IPersistentConnection> connections)
        {
            foreach (var connection in connections)
            {
                connection.LastCommitTime = LastCommitTime;
                CommitConnections[connection.DeviceId] = connection;
            }
        }
        public bool IsCommitable(string deviceId)
        {
            IPersistentConnection connection;
            if (CommitConnections.TryGetValue(deviceId, out connection))
            {
                return connection.LastCommitTime > _dateTimeProvider.UtcNow;
            }
            return false;
        }

        public void CommitSuccess(string deviceId)
        {
            IPersistentConnection connection;
            if (CommitConnections.TryRemove(deviceId, out connection))
            {
                connection.NextPeekTime = PeekTime;
                PeekConnections.Enqueue(connection);
            }
        }

        private DateTime ReceiveAndForgetTime
        {
            get { return _dateTimeProvider.UtcNow.AddSeconds(1.0); }
        }

        private DateTime PeekTime
        {
            get { return _dateTimeProvider.UtcNow.AddSeconds(1.0); }
        }

        private DateTime LastCommitTime
        {
            get { return _dateTimeProvider.UtcNow.AddSeconds(10.0); }
        }

        private TimeSpan PutBackToPeekInteval
        {
            get { return TimeSpan.FromSeconds(5.0); }
        }

        private ConcurrentQueue<IPersistentConnection> ReceiveAndForgetConnections { get; set; }

        private ConcurrentQueue<IPersistentConnection> PeekConnections { get; set; }

        private ConcurrentDictionary<string, IPersistentConnection> CommitConnections { get; set; }
    }
}
