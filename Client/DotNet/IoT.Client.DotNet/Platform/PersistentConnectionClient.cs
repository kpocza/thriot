using System;
using System.Threading;
using IoT.Client.DotNet.Platform.Exceptions;

namespace IoT.Client.DotNet.Platform
{
    public class PersistentConnectionClient
    {
        private PersistentConnectionInternalClient _persistentConnectionInternalClient;
        private bool _isLoggedIn;
        private bool _isSubscribed;
        private string _url;
        private string _deviceId;
        private string _apiKey;
        private SubscriptionType _subscriptionType;
        private Action<PushedMessage> _onMessageReceived;
        private readonly int _maxRetryCount;

        public PersistentConnectionClient(int maxRetryCount = 5)
        {
            InitializeClient();
            _isLoggedIn = false;
            _isSubscribed = false;
            _maxRetryCount = maxRetryCount;
        }

        public void Login(string url, string deviceId, string apiKey)
        {
            _isLoggedIn = true;
            _url = url;
            _deviceId = deviceId;
            _apiKey = apiKey;

            Exception lastException = null;
            int retryCount = 0;
            while (retryCount < _maxRetryCount)
            {
                try
                {
                    _persistentConnectionInternalClient.Login(url, deviceId, apiKey);
                    return;
                }
                catch (DisconnectedException ex)
                {
                    Wait();
                    InitializeClient();
                    lastException = ex;
                }
                catch (ConnectedAlreadyException ex)
                {
                    Wait();
                    InitializeClient();
                    lastException = ex;
                }
                catch (ConnectionFailedException ex)
                {
                    Wait();
                    InitializeClient();
                    lastException = ex;
                }
                catch (TimeoutException ex)
                {
                    Wait();
                    InitializeClient();
                    lastException = ex;
                }
                retryCount++;
            }

            throw lastException;
        }

        public void Subscribe(SubscriptionType subscriptionType, Action<PushedMessage> onMessageReceived)
        {
            _isSubscribed = true;
            _subscriptionType = subscriptionType;
            _onMessageReceived = onMessageReceived;

            Exception lastException = null;
            int retryCount = 0;
            while (retryCount < _maxRetryCount)
            {
                try
                {
                    _persistentConnectionInternalClient.Subscribe(subscriptionType, onMessageReceived);
                    return;
                }
                catch (DisconnectedException ex)
                {
                    Wait();
                    Relogin();
                    lastException = ex;
                }
                catch (LoginRequiredException ex)
                {
                    Wait();
                    Relogin();
                    lastException = ex;
                }
                catch (TimeoutException ex)
                {
                    Wait();
                    lastException = ex;
                }
                catch (SubscribedAlreadyException)
                {
                    return;
                }
                retryCount++;
            }

            throw lastException;
        }

        public void Unsubscribe()
        {
            _isSubscribed = false;

            Exception lastException = null;
            int retryCount = 0;
            while (retryCount < _maxRetryCount)
            {
                try
                {
                    _persistentConnectionInternalClient.Unsubscribe();
                    return;
                }
                catch (TimeoutException ex)
                {
                    Wait();
                    lastException = ex;
                }
                retryCount++;
            }

            throw lastException;
        }

        public void Close()
        {
            _persistentConnectionInternalClient.Close();

            _isLoggedIn = false;
            _isSubscribed = false;
        }

        public void RecordTelemetryData(string payload)
        {
            Exception lastException = null;
            int retryCount = 0;
            while (retryCount < _maxRetryCount)
            {
                try
                {
                    _persistentConnectionInternalClient.RecordTelemetryData(payload);
                    return;
                }
                catch (DisconnectedException ex)
                {
                    Wait();
                    Relogin();
                    lastException = ex;
                }
                catch (LoginRequiredException ex)
                {
                    Wait();
                    Relogin();
                    lastException = ex;
                }
                catch (TimeoutException ex)
                {
                    Wait();
                    lastException = ex;
                }
                retryCount++;
            }

            throw lastException;
        }

        public void SendMessageTo(string deviceId, string payload)
        {
            Exception lastException = null;
            int retryCount = 0;
            while (retryCount < _maxRetryCount)
            {
                try
                {
                    _persistentConnectionInternalClient.SendMessageTo(deviceId, payload);
                    return;
                }
                catch (DisconnectedException ex)
                {
                    Wait();
                    Relogin();
                    lastException = ex;
                }
                catch (LoginRequiredException ex)
                {
                    Wait();
                    Relogin();
                    lastException = ex;
                }
                catch (TimeoutException ex)
                {
                    Wait();
                    lastException = ex;
                }
                retryCount++;
            }

            throw lastException;
        }

        private void InitializeClient()
        {
            _persistentConnectionInternalClient = new PersistentConnectionInternalClient(new WebSocketConnection());
        }

        private void Relogin()
        {
            if (_isLoggedIn)
            {
                InitializeClient();
                Login(_url, _deviceId, _apiKey);
            }
        }

        private static void Wait()
        {
            Thread.Sleep(10);
        }
    }
}
