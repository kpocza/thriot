using System;

namespace IoT.Client.DotNet.Platform
{
    public interface IWebSocketConnection : IDisposable
    {
        void Connect(string uri);

        void Send(string message, bool wait = true);

        void Close();

        bool IsDisconnected { get; }

        WebSocketConnection OnMessage(Action<string, WebSocketConnection> onMessage);
    }
}