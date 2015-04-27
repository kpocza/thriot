using System;

namespace Thriot.Platform.PersistentConnections
{
    [Flags]
    public enum ConnectionState
    {
        None = 0,
        Initiated = 1,
        LoggedIn = 2,
        Subscribed = 4,
    }
}
