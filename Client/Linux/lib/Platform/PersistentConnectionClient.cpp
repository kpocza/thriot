#include "PlatformClient.h"
#include "../WebSocketConnection.h"
#include "PersistentConnectionInternalClient.h"
#include <unistd.h>
#include <ctime>

namespace Thriot { namespace Platform {

/**
Create a new instance of persistently connected client.

@param url Base API Url 
@param maxRetryCount Maximum retry count for operations (default is 5)
*/
PersistentConnectionClient::PersistentConnectionClient(const string& url, const int maxRetryCount):_heartbeatTimeSpan(60)
{
	_isLoggedIn = false;
	_isSubscribed = false;
	_url = url;
	_maxRetryCount = maxRetryCount;
	InitializeClient();
}

/**
Login the device. All subsequent operations will be accomplished in the name of the connected client.

@param deviceId 32-characters long unique device id 
@param apiKey Access key of the device or the enclosing network or service

@return Ok or any of the error codes (Disconnected, ConnectedAlready, ConnectionFailed, Timeout, LoginInvalid, etc.) */
PlatformOperationResult PersistentConnectionClient::Login(const string& deviceId, const string& apiKey)
{
	_isLoggedIn = true;
	_deviceId = deviceId;
	_apiKey = apiKey;

	PlatformOperationResult lastResult = Ok;
	int retryCount = 0;
	while (retryCount < _maxRetryCount)
	{
		lastResult = _persistentConnectionInternalClient->Login(deviceId, apiKey);

		switch(lastResult)
		{
			case Disconnected:
				Wait();
				InitializeClient();
				break;

			case ConnectedAlready:
				Wait();
				InitializeClient();
				break;

			case ConnectionFailed:
				Wait();
				InitializeClient();
				break;

			case Timeout:
				Wait();
				InitializeClient();
				break;

			case Ok:
				RecordHeartbeat();

			default: 
				return lastResult;
		}
		retryCount++;
	}

	return lastResult;
}

/**
Subscribe to the message channel of the device with the given subscription type (QoS level).
The onMessageReceived callback will be executed on message receive. The commit operation is automatically executed by the client
in case of QoS 1.

@param subscriptionType ReceiveAndForget or ReceiveAndCommit
@param onMessageReceived Method to be executed for message processing

@return Ok or error code (Disconnected, LoginRequired, Timeout, SubscribeInvalid, LoginRequired, etc.) */
PlatformOperationResult PersistentConnectionClient::Subscribe(const SubscriptionType subscriptionType, OnMessageReceived onMessageReceived)
{
	_isSubscribed = true;
	_subscriptionType = subscriptionType;
	_onMessageReceived = onMessageReceived;

	PlatformOperationResult lastResult = Ok;
	int retryCount = 0;
	while (retryCount < _maxRetryCount)
	{
	
		lastResult = _persistentConnectionInternalClient->Subscribe(subscriptionType, onMessageReceived);

		switch(lastResult)
		{
			case Disconnected:
				Wait();
				Relogin();
				break;

			case LoginRequired:
				Wait();
				Relogin();
				break;

			case Timeout:
				Wait();
				break;

			case SubscribedAlready:
				RecordHeartbeat();
				return Ok;

			case Ok:
				RecordHeartbeat();

			default:
				return lastResult;
		}

		retryCount++;
	}

	return lastResult;
}

/**
Unsubscribe from an already subscribed device channel.

@return Ok or error code (TimeOut, SubscribeRequired, LoginRequired, etc)*/
PlatformOperationResult PersistentConnectionClient::Unsubscribe()
{
	_isSubscribed = false;

	PlatformOperationResult lastResult = Ok;
	int retryCount = 0;
	while (retryCount < _maxRetryCount)
	{
		lastResult = _persistentConnectionInternalClient->Unsubscribe();

		if(lastResult == Timeout)
		{
			Wait();
		}
		else
		{
			if(lastResult == Ok)
				RecordHeartbeat();

			return lastResult;
		}
		retryCount++;
	}
	return lastResult;
}

/**
Close connection
*/
void PersistentConnectionClient::Close()
{
	_persistentConnectionInternalClient->Close();

	_isLoggedIn = false;
	_isSubscribed = false;
}

/**
Record telemetry data in the name of the currently logged in device.

@param payload Message payload. Maximum 1024 characters long.

@return Ok or error code (LoginRequired, Disconnected, Timeout, TelemetryDataInvalid, etc.) */
PlatformOperationResult PersistentConnectionClient::RecordTelemetryData(const string& payload)
{
	PlatformOperationResult lastResult = Ok;
	int retryCount = 0;
	while (retryCount < _maxRetryCount)
	{
		lastResult = _persistentConnectionInternalClient->RecordTelemetryData(payload);

		switch(lastResult)
		{
			case Disconnected:
				Wait();
				Relogin();
				break;

			case LoginRequired:
				Wait();
				Relogin();
				break;

			case Timeout:
				Wait();
				break;

			case Ok:
				RecordHeartbeat();

			default:
				return lastResult;
		}

		retryCount++;
	}

	throw lastResult;
}

/**
Send message from the logged in device to an other device in the same network

@param deviceId Target device id
@param payload Message to be sent (512 characters max)

@return Ok or error code (Disconnected, LoginRequired, Timeout, SendToInvalid, etc.) */
PlatformOperationResult PersistentConnectionClient::SendMessageTo(const string& deviceId, const string& payload)
{
	PlatformOperationResult lastResult = Ok;
	int retryCount = 0;
	while (retryCount < _maxRetryCount)
	{
		lastResult = _persistentConnectionInternalClient->SendMessageTo(deviceId, payload);

		switch(lastResult)
		{
			case Disconnected:
				Wait();
				Relogin();
				break;

			case LoginRequired:
				Wait();
				Relogin();
				break;

			case Timeout:
				Wait();
				break;

			case Ok:
				RecordHeartbeat();

			default:
				return lastResult;
		}
		retryCount++;
	}

	return lastResult;
}

/**
Let the background websocket work. Call this operation regularly in a while loop in the main processing cycle of your application
to be able to send and receive websocket message in the background.

@return Ok or error code
*/
PlatformOperationResult PersistentConnectionClient::Spin()
{
	if(_lastHeartbeatTime + _heartbeatTimeSpan < std::time(0))
	{
		PlatformOperationResult result = _persistentConnectionInternalClient->Heartbeat();
		if(result!= Ok)
			return result;

		RecordHeartbeat();
	}
	_persistentConnectionInternalClient->Spin();

	return Ok;
}

void PersistentConnectionClient::RecordHeartbeat()
{
	_lastHeartbeatTime = std::time(0);
}

void PersistentConnectionClient::InitializeClient()
{
	_persistentConnectionInternalClient = new PersistentConnectionInternalClient(_url, new WebSocketConnection());
}

void PersistentConnectionClient::Relogin()
{
	if (_isLoggedIn)
	{
		InitializeClient();
		Login(_deviceId, _apiKey);
	}
}

void PersistentConnectionClient::Wait()
{
	usleep(10 * 1000);
}
}}

