using System;
using IoT.Platform.Model.Messaging;

namespace IoT.Platform.PersistentConnections
{
    public interface IPersistentConnection
    {
        Guid ConnectionId { get; }

        string DeviceId { get; set; }

        long NumericDeviceId { get; set; }

        ConnectionState ConnectionState { get; set; }

        SubscriptionType SubscriptionType { get; set; }

        DateTime LastHeartbeat { get; }

        DateTime LastPing { get; }

        DateTime NextReceiveAndForgetTime { get; set; }

        DateTime NextPeekTime { get; set; }

        DateTime LastCommitTime { get; set; }

        void Reply(string response);

        void Close();

        void Heartbeat();

        void Ping();

        void SendMessage(OutgoingMessageToStoreWithState msg);
    }
}
