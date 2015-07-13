using SuperSocket.WebSocket;
using System;
using Thriot.Platform.PersistentConnections;

namespace Thriot.Platform.WebsocketService
{
    class IotWebSocketServer : WebSocketServer<IotSession>
    {
        private ConnectionRegistry _connectionRegistry;
        private Func<CommandExecutor> _commandExecutor;

        public void SetConnectionRegistry(ConnectionRegistry connectionRegistry, Func<CommandExecutor> commandExecutor)
        {
            _connectionRegistry = connectionRegistry;
            _commandExecutor = commandExecutor;
        }

        protected override void OnNewSessionConnected(IotSession session)
        {
            session.SetConnectionRegistry(_connectionRegistry, _commandExecutor);
            base.OnNewSessionConnected(session);
        }
    }
}