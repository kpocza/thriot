using System;
using System.Threading;
using Thriot.Client.DotNet.Platform.Exceptions;

namespace Thriot.Client.DotNet.Platform
{
    internal class PersistentConnectionInternalClient
    {
        private readonly IWebSocketConnection _webSocketConnection;
        private bool _isLoggedIn;
        private bool _isSubscribed;
        private SubscriptionType _subscriptionType;
        
        private Action<PushedMessage> _onMessageReceived;
        
        private readonly AutoResetEvent _responseEvent;
        private string _lastResponse;

        public PersistentConnectionInternalClient(IWebSocketConnection webSocketConnection)
        {
            _webSocketConnection = webSocketConnection;
            _isLoggedIn = false;
            _isSubscribed = false;
            _responseEvent = new AutoResetEvent(false);
        }

        public void Login(string url, string deviceId, string apiKey)
        {
            if(_isLoggedIn)
                throw new InvalidOperationException("Already logged in");

            _webSocketConnection.Connect(url);
            _webSocketConnection.OnMessage(MessagePayloadReceived);

            if(_webSocketConnection.IsDisconnected)
                throw new DisconnectedException();

            _webSocketConnection.Send(string.Format("login {0} {1}", deviceId, apiKey));

            if (_responseEvent.WaitOne(TimeSpan.FromSeconds(10)))
            {
                if (_lastResponse == "login ack")
                {
                    _isLoggedIn = true;
                }
                else
                {
                    throw new LoginInvalidException(_lastResponse);
                }
            }
            else
            {
                throw new TimeoutException("Login timeout expired");
            }
        }

        public void Subscribe(SubscriptionType subscriptionType, Action<PushedMessage> onMessageReceived)
        {
            LoginRequired();

            if(_isSubscribed)
                throw new InvalidOperationException("Already subscribed");

            if (onMessageReceived == null)
                throw new ArgumentNullException("onMessageReceived");

            if (_webSocketConnection.IsDisconnected)
                throw new DisconnectedException();

            _subscriptionType = subscriptionType;
            if (subscriptionType == SubscriptionType.ReceiveAndForget)
            {
                _webSocketConnection.Send("subscribe receiveandforget");
            }
            else if (subscriptionType == SubscriptionType.PeekAndCommit)
            {
                _webSocketConnection.Send("subscribe peekandcommit");
            }
            else
            {
                throw new ArgumentOutOfRangeException("subscriptionType");
            }

            if (_responseEvent.WaitOne(TimeSpan.FromSeconds(10)))
            {
                if (_lastResponse == "subscribe ack")
                {
                    _isSubscribed = true;
                    _onMessageReceived = onMessageReceived;
                    return;
                }

                if (_lastResponse == "subscribe unauthorized")
                    throw new LoginRequiredException();

                if (_lastResponse == "subscribe already")
                    throw new SubscribedAlreadyException();

                throw new SubscribeInvalidException(_lastResponse);
            }
            else
            {
                throw new TimeoutException("Subscribe timeout expired");
            }
        }

        public void Unsubscribe()
        {
            LoginAndSubscriptionRequired();

            if (_webSocketConnection.IsDisconnected)
                throw new DisconnectedException();
            
            _webSocketConnection.Send("unsubscribe");

            if (_responseEvent.WaitOne(TimeSpan.FromSeconds(10)))
            {
                if (_lastResponse == "unsubscribe ack")
                {
                    _isSubscribed = false;
                    return;
                }

                if (_lastResponse == "unsubscribe unauthorized")
                    throw new LoginRequiredException();

                if (_lastResponse == "unsubscribe notsubscribed")
                    throw new UnsubscribeNotsubscribedException();

                throw new UnsubscribeInvalidException(_lastResponse);
            }
            else
            {
                throw new TimeoutException("Unsubscribe timeout expired");
            }
        }

        public void Close()
        {
            LoginRequired();
            
            _webSocketConnection.Send("close", false);

            _isLoggedIn = false;
            _isSubscribed = false;

            _webSocketConnection.Close();
        }

        public void RecordTelemetryData(string payload)
        {
            LoginRequired();

            if (_webSocketConnection.IsDisconnected)
                throw new DisconnectedException();
            
            _webSocketConnection.Send("telemetrydata " + payload);

            if (_responseEvent.WaitOne(TimeSpan.FromSeconds(10)))
            {
                if (_lastResponse == "telemetrydata ack")
                    return;

                if (_lastResponse == "telemetrydata unauthorized")
                    throw new LoginRequiredException();

                throw new TelemetryDataInvalidException(_lastResponse);
            }
            else
            {
                throw new TimeoutException("RecordTelemetryData timeout expired");
            }
        }

        public void SendMessageTo(string deviceId, string payload)
        {
            LoginRequired();

            if (_webSocketConnection.IsDisconnected)
                throw new DisconnectedException();
            
            _webSocketConnection.Send("sendto " + deviceId + " " + payload);

            if (_responseEvent.WaitOne(TimeSpan.FromSeconds(10)))
            {
                if (_lastResponse == "sendto ack")
                    return;

                if (_lastResponse == "sendto unauthorized")
                    throw new LoginRequiredException();

                throw new SendToInvalidException(_lastResponse);
            }
            else
            {
                throw new TimeoutException("Sendto timeout expired");
            }
        }

        private void MessagePayloadReceived(string msg, WebSocketConnection connection)
        {
            if (msg == "yo")
            {
                try
                {
                    _webSocketConnection.Send("heartbeat");
                }
                catch
                {
                    // if not succeeds then may reconnect
                }
                return;
            }
            if (msg.StartsWith("pushedmessage "))
            {
                HandlePushedMessage(msg);
                return;
            }

            _lastResponse = msg;
            _responseEvent.Set();
        }

        private void HandlePushedMessage(string msg)
        {
            ProcessMessage(msg);

            CommitIfNeded();
        }

        private void ProcessMessage(string msg)
        {
            var parts = msg.Split(new[] {' '}, 5);

            if (parts.Length < 3)
                throw new ArgumentException("pushedMessage");

            var messageIdText = parts[1];
            int messageId;

            if (!int.TryParse(messageIdText, out messageId))
                throw new ArgumentException("Invalid messageid");

            var timestampText = parts[2];
            long timestamp;

            if (!long.TryParse(timestampText, out timestamp))
                throw new ArgumentException("Invalid timespan");

            string senderDeviceId = parts[3];

            var messageText = msg.Substring(parts[0].Length + parts[1].Length + parts[2].Length + parts[3].Length + 4);

            _onMessageReceived(new PushedMessage(messageId, timestamp, messageText, senderDeviceId));
        }

        private void CommitIfNeded()
        {
            if (_subscriptionType == SubscriptionType.PeekAndCommit)
            {
                try
                {
                    _webSocketConnection.Send("commit");
                }
                catch
                {
                    // if not succeeds then peek the message again, do not care
                }
            }
        }

        private void LoginAndSubscriptionRequired()
        {
            if (!_isLoggedIn)
                throw new LoginRequiredException();

            if (!_isSubscribed)
                throw new SubscriptionRequiredException();
        }

        private void LoginRequired()
        {
            if (!_isLoggedIn)
                throw new LoginRequiredException();
        }
    }
}
