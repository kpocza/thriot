using System;
using System.Threading;
using Thriot.Client.DotNet.Platform.Exceptions;

namespace Thriot.Client.DotNet.Platform
{
    /// <summary>
    /// Represents a persistent connection to the service implemented by Websockets.
    /// </summary>
    public class PersistentConnectionClient
    {
        private readonly int _maxRetryCount;
        private readonly string _url;
        private PersistentConnectionInternalClient _persistentConnectionInternalClient;
        private bool _isLoggedIn;
        private string _deviceId;
        private string _apiKey;
        private SubscriptionType _subscriptionType;
        private Action<PushedMessage> _onMessageReceived;

        private static readonly TimeSpan _heatbeatTimespan = TimeSpan.FromMinutes(1.0);
        private DateTime _lastHeartbeatTime;

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="url">Websocket api root url</param>
        /// <param name="maxRetryCount">Max retry count. Default value is 5.</param>
        public PersistentConnectionClient(string url, int maxRetryCount = 5)
        {
            InitializeClient();
            _url = url;
            _isLoggedIn = false;
            _maxRetryCount = maxRetryCount;
        }

        /// <summary>
        /// Logs in a device to the service and open a persistent websocket connection
        /// </summary>
        /// <param name="deviceId">Unique device identifier</param>
        /// <param name="apiKey">Api key of the device, or any of the enclosing networks, or the enclosing service</param>
        /// <exception cref="DisconnectedException">Exception is thrown when the client is accidentally disconnected from the service</exception>
        /// <exception cref="ConnectedAlreadyException">Exception is thrown when the connection is alrady opened</exception>
        /// <exception cref="ConnectionFailedException">Exception is thrown when there is some error while connecting</exception>
        /// <exception cref="TimeoutException">Timeout period exceeded</exception>
        /// <exception cref="TooManyRetriesException">The retry count exceeded</exception>
        public void Login(string deviceId, string apiKey)
        {
            _isLoggedIn = true;
            _deviceId = deviceId;
            _apiKey = apiKey;

            Exception lastException = new TooManyRetriesException();
            int retryCount = 0;
            while (retryCount < _maxRetryCount)
            {
                try
                {
                    _persistentConnectionInternalClient.Login(_url, deviceId, apiKey);
                    RecordHeartbeat();
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

        /// <summary>
        /// Subscribe to be able to receive pushed message from the service. When the device is not supposed to receive messages (only records telemetry data) this method shouldn't be called.
        /// Then the onMessageReceived action is successfully executed in QoS 1 mode the message is automatically commited.
        /// 
        /// </summary>
        /// <param name="subscriptionType">QoS 0 or QoS 1-level subscription</param>
        /// <param name="onMessageReceived">This action is called whena message is received</param>
        /// <exception cref="DisconnectedException">Exception is thrown when the client is accidentally disconnected from the service</exception>
        /// <exception cref="LoginRequiredException">Exception is thrown when the device is not logged in to the service</exception>
        /// <exception cref="SubscribedAlreadyException">The device is already subscribed for pushed messages</exception>
        /// <exception cref="TimeoutException">Timeout period exceeded</exception>
        /// <exception cref="TooManyRetriesException">The retry count exceeded</exception>
        public void Subscribe(SubscriptionType subscriptionType, Action<PushedMessage> onMessageReceived)
        {
            _subscriptionType = subscriptionType;
            _onMessageReceived = onMessageReceived;

            Exception lastException = new TooManyRetriesException();
            int retryCount = 0;
            while (retryCount < _maxRetryCount)
            {
                try
                {
                    _persistentConnectionInternalClient.Subscribe(subscriptionType, onMessageReceived);
                    RecordHeartbeat();
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
                    RecordHeartbeat();
                    return;
                }
                retryCount++;
            }

            throw lastException;
        }

        /// <summary>
        /// Unsubscribe the device from automatically receiving pushed messages
        /// </summary>
        /// <exception cref="DisconnectedException">Exception is thrown when the client is accidentally disconnected from the service</exception>
        /// <exception cref="LoginRequiredException">Exception is thrown when the device is not logged in to the service</exception>
        /// <exception cref="UnsubscribeNotsubscribedException">The device was not subscribed so it cannot unsubscribe</exception>
        /// <exception cref="UnsubscribeInvalidException">Error occured while unsubscribe</exception>
        /// <exception cref="TimeoutException">Timeout period exceeded</exception>
        /// <exception cref="TooManyRetriesException">The retry count exceeded</exception>
        public void Unsubscribe()
        {
            Exception lastException = new TooManyRetriesException();
            int retryCount = 0;
            while (retryCount < _maxRetryCount)
            {
                try
                {
                    _persistentConnectionInternalClient.Unsubscribe();
                    RecordHeartbeat();
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

        /// <summary>
        /// Close connection
        /// </summary>
        public void Close()
        {
            _persistentConnectionInternalClient.Close();

            _isLoggedIn = false;
        }

        /// <summary>
        /// Record a telemetry data payload
        /// </summary>
        /// <param name="payload">Telemetry data payload</param>
        /// <exception cref="DisconnectedException">Exception is thrown when the client is accidentally disconnected from the service</exception>
        /// <exception cref="LoginRequiredException">Exception is thrown when the device is not logged in to the service</exception>
        /// <exception cref="TelemetryDataInvalidException">Tried to send invalid telemetry data</exception>
        /// <exception cref="TimeoutException">Timeout period exceeded</exception>
        /// <exception cref="TooManyRetriesException">The retry count exceeded</exception>
        public void RecordTelemetryData(string payload)
        {
            Exception lastException = new TooManyRetriesException();
            int retryCount = 0;
            while (retryCount < _maxRetryCount)
            {
                try
                {
                    _persistentConnectionInternalClient.RecordTelemetryData(payload);
                    RecordHeartbeat();
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

        /// <summary>
        /// Send message to a device. The target device must be in the same network as the sender device.
        /// </summary>
        /// <param name="deviceId">Target device</param>
        /// <param name="payload">Message payload</param>
        /// <exception cref="DisconnectedException">Exception is thrown when the client is accidentally disconnected from the service</exception>
        /// <exception cref="LoginRequiredException">Exception is thrown when the device is not logged in to the service</exception>
        /// <exception cref="SendToInvalidException">Error while sending message</exception>
        /// <exception cref="TimeoutException">Timeout period exceeded</exception>
        /// <exception cref="TooManyRetriesException">The retry count exceeded</exception>
        public void SendMessageTo(string deviceId, string payload)
        {
            Exception lastException = new TooManyRetriesException();
            int retryCount = 0;
            while (retryCount < _maxRetryCount)
            {
                try
                {
                    _persistentConnectionInternalClient.SendMessageTo(deviceId, payload);
                    RecordHeartbeat();
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

        public void Spin()
        {
            if (_lastHeartbeatTime + _heatbeatTimespan < DateTime.UtcNow)
            {
                _persistentConnectionInternalClient.Heartbeat();
                RecordHeartbeat();
            }
            Thread.Sleep(1);
        }

        private void RecordHeartbeat()
        {
            _lastHeartbeatTime = DateTime.UtcNow;
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
                Login(_deviceId, _apiKey);
            }
        }

        private static void Wait()
        {
            Thread.Sleep(10);
        }
    }
}
