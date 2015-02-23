#include "PlatformClient.h"
#include "../WebSocketConnection.h"
#include "PersistentConnectionInternalClient.h"
#include <unistd.h>

PersistentConnectionClient::PersistentConnectionClient(const int maxRetryCount)
{
	InitializeClient();
	_isLoggedIn = false;
	_isSubscribed = false;
	_maxRetryCount = maxRetryCount;
}

PlatformOperationResult PersistentConnectionClient::Login(const string& url, const string& deviceId, const string& apiKey)
{
	_isLoggedIn = true;
	_url = url;
	_deviceId = deviceId;
	_apiKey = apiKey;

	PlatformOperationResult lastResult = Ok;
	int retryCount = 0;
	while (retryCount < _maxRetryCount)
	{
		lastResult = _persistentConnectionInternalClient->Login(url, deviceId, apiKey);

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

			default: 
				return lastResult;
		}
		retryCount++;
	}

	return lastResult;
}

PlatformOperationResult PersistentConnectionClient::Subscribe(const SubscriptionType subscriptionType, 
													OnMessageReceived onMessageReceived)
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
				return Ok;

			default:
				return lastResult;
		}

	    retryCount++;
	}

	return lastResult;
}

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
			return lastResult;
		}
		retryCount++;
	}
	return lastResult;
}

void PersistentConnectionClient::Close()
{
	_persistentConnectionInternalClient->Close();

	_isLoggedIn = false;
	_isSubscribed = false;
}

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

			default:
				return lastResult;
		}

	    retryCount++;
	}

	throw lastResult;
}

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

		    case LoginRequired:
		        Wait();
				Relogin();

		    case Timeout:
		        Wait();

			default:
				return lastResult;
		}
	    retryCount++;
	}

	return lastResult;
}

void PersistentConnectionClient::Spin()
{
	_persistentConnectionInternalClient->Spin();
}

void PersistentConnectionClient::InitializeClient()
{
	_persistentConnectionInternalClient = new PersistentConnectionInternalClient(new WebSocketConnection());
}

void PersistentConnectionClient::Relogin()
{
	if (_isLoggedIn)
	{
		InitializeClient();
		Login(_url, _deviceId, _apiKey);
	}
}

void PersistentConnectionClient::Wait()
{
	usleep(10 * 1000);
}

