using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using Thriot.Client.DotNet.Platform.Exceptions;

namespace Thriot.Client.DotNet.Platform
{
    public class WebSocketConnection : IWebSocketConnection
    {
        private const int ReceiveChunkSize = 1024;
        private const int SendChunkSize = 1024;

        private readonly ClientWebSocket _clientWebSocket;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationToken _cancellationToken;
        private bool _isConnected;
        private Action<string, WebSocketConnection> _onMessage;

        public WebSocketConnection()
        {
            _clientWebSocket = new ClientWebSocket();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _isConnected = false;
        }

        public void Connect(string uri)
        {
            if (_isConnected)
                throw new ConnectedAlreadyException();

            _clientWebSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(20);

            try
            {
                if (
                    !_clientWebSocket.ConnectAsync(new Uri(uri), _cancellationToken)
                        .Wait(TimeSpan.FromMilliseconds(30000)))
                    throw new InvalidOperationException("Connection failed");
            }
            catch
            {
                _isConnected = false;
                _clientWebSocket.Dispose();

                throw new ConnectionFailedException();
                // try again
            }
            _isConnected = true;

            StartListen();
        }

        public void Send(string message, bool wait = true)
        {
            if (_clientWebSocket.State != WebSocketState.Open)
            {
                throw new Exception("Connection is not open.");
            }

            var messageBuffer = Encoding.UTF8.GetBytes(message);
            var messagesCount = (int)Math.Ceiling((double)messageBuffer.Length / SendChunkSize);

            for (var i = 0; i < messagesCount; i++)
            {
                var offset = (SendChunkSize * i);
                var count = SendChunkSize;
                var lastMessage = ((i + 1) == messagesCount);

                if ((count * (i + 1)) > messageBuffer.Length)
                {
                    count = messageBuffer.Length - offset;
                }

                var task = _clientWebSocket.SendAsync(new ArraySegment<byte>(messageBuffer, offset, count), WebSocketMessageType.Text, lastMessage, _cancellationToken);
                if (wait)
                    task.Wait();
            }
        }

        public void Close()
        {
            _cancellationTokenSource.Cancel();
            _isConnected = false;
        }

        public bool IsDisconnected
        {
            get { return _clientWebSocket.State != WebSocketState.Open; }
        }

        public WebSocketConnection OnMessage(Action<string, WebSocketConnection> onMessage)
        {
            _onMessage = onMessage;
            return this;
        }

        private async void StartListen()
        {
            var buffer = new byte[ReceiveChunkSize];

            try
            {
                while (_clientWebSocket.State == WebSocketState.Open)
                {
                    var stringResult = new StringBuilder();

                    WebSocketReceiveResult result;
                    do
                    {
                        result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationToken);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await
                                _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                            CallOnDisconnected();

                            _clientWebSocket.Dispose();
                            return;
                        }
                        else
                        {
                            var str = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            stringResult.Append(str);
                        }

                    } while (!result.EndOfMessage);

                    CallOnMessage(stringResult.ToString());
                }
            }
            catch (Exception)
            {
                CallOnDisconnected();
            }
            finally
            {
                _clientWebSocket.Dispose();
            }
        }

        private void CallOnMessage(string stringResult)
        {
            if (_onMessage != null)
                _onMessage(stringResult, this);
        }

        private void CallOnDisconnected()
        {
            _isConnected = false;
            _clientWebSocket.Dispose();
        }

        public void Dispose()
        {
            _clientWebSocket.Dispose();
        }
    }
}
