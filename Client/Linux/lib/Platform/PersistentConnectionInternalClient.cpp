#include "PersistentConnectionInternalClient.h"
#include "../WebSocketConnection.h"
#include <cstdlib>

namespace Thriot { namespace Platform {

PersistentConnectionInternalClient::PersistentConnectionInternalClient(const string& url, WebSocketConnection* webSocketConnection)
{
	_url = url;
	_webSocketConnection = webSocketConnection;
	_isLoggedIn = false;
	_isSubscribed = false;
}

PlatformOperationResult PersistentConnectionInternalClient::Login(const string& deviceId, const string& apiKey)
{
	if(_isLoggedIn)
		return InvalidOperation;

	if(!_webSocketConnection->IsDisconnected())
		return ConnectedAlready;

	if(!_webSocketConnection->Connect(_url))
		return Disconnected;

	_webSocketConnection->OnMessage(this, OnMessagePayloadReceived);

	if(_webSocketConnection->IsDisconnected())
		return ConnectionFailed;

	_webSocketConnection->Send("login " + deviceId + " " + apiKey);

	string response = _webSocketConnection->WaitResponse(10);

	if(!response.empty())
	{
		if(response == "login ack")
		{
			_isLoggedIn = true;
			return Ok;
		}
		else
		{
			return LoginInvalid;
		}
	}
	else
	{
		return Timeout;
	}
}

PlatformOperationResult PersistentConnectionInternalClient::Subscribe(
				const SubscriptionType subscriptionType, OnMessageReceived onMessageReceived)
{
	if(!_isLoggedIn)
		return LoginRequired;

	if(_isSubscribed)
		return InvalidOperation;

	if(_webSocketConnection->IsDisconnected())
		return Disconnected;

	_subscriptionType = subscriptionType;
	if(subscriptionType == ReceiveAndForget)
	{
		_webSocketConnection->Send("subscribe receiveandforget");
	}
	else if(subscriptionType == PeekAndCommit)
	{
		_webSocketConnection->Send("subscribe peekandcommit");
	}
	else
	{
		return ArgumentOutOfRange;
	}

	string response = _webSocketConnection->WaitResponse(10);

	if(!response.empty())
	{
		if(response == "subscribe ack")
		{
			_isSubscribed = true;
			_onMessageReceived = onMessageReceived;
			return Ok;
		}

		if(response == "subscribe unauthorized")
			return LoginRequired;

		if(response == "subscribe already")
			return SubscribedAlready;

		return SubscribeInvalid;
	}
	else
	{
		return Timeout;
	}
}

PlatformOperationResult PersistentConnectionInternalClient::Unsubscribe()
{
	if(!_isLoggedIn)
		return LoginRequired;

	if(!_isSubscribed)
		return SubscriptionRequired;

	if(_webSocketConnection->IsDisconnected())
		return Disconnected;

	_webSocketConnection->Send("unsubscribe");
	
	string response = _webSocketConnection->WaitResponse(10);

	if(!response.empty())
	{
		if(response == "unsubscribe ack")
		{
			_isSubscribed = false;
			return Ok;
		}

		if(response == "unsubscribe unauthorized")
			return LoginRequired;

		if(response == "unsubscribe notsubscribed")
			return UnsubscribeNotsubscribed;

		return UnsubscribeInvalid;
	}
	else
	{
		return Timeout;
	}
}

PlatformOperationResult PersistentConnectionInternalClient::Close()
{
	if(!_isLoggedIn)
		return LoginRequired;

	_webSocketConnection->Send("close");

	_isLoggedIn = false;
	_isSubscribed = false;

	_webSocketConnection->Close();

	return Ok;
}

PlatformOperationResult PersistentConnectionInternalClient::RecordTelemetryData(const string& payload)
{
	if(!_isLoggedIn)
		return LoginRequired;

	if(_webSocketConnection->IsDisconnected())
		return Disconnected;

	_webSocketConnection->Send("telemetrydata " + payload);

	string response = _webSocketConnection->WaitResponse(10);

	if(!response.empty())
	{
		if(response == "telemetrydata ack")
			return Ok;

		if(response == "telemetrydata unauthorized")
			return LoginRequired;

		return TelemetryDataInvalid;
	}
	else
	{
		return Timeout;
	}
}

PlatformOperationResult PersistentConnectionInternalClient::SendMessageTo(const string& deviceId, const string& payload)
{
	if(!_isLoggedIn)
		return LoginRequired;

	if(_webSocketConnection->IsDisconnected())
		return Disconnected;

	_webSocketConnection->Send("sendto " + deviceId + " " + payload);

	string response = _webSocketConnection->WaitResponse(10);

	if(!response.empty())
	{
		if(response == "sendto ack")
			return Ok;

		if(response == "sendto unauthorized")
			return LoginRequired;

		return SendToInvalid;
	}
	else
	{
		return Timeout;
	}
}

void PersistentConnectionInternalClient::Spin()
{
	_webSocketConnection->Spin();
}

void PersistentConnectionInternalClient::HandlePushedMessage(const string& message)
{
	ProcessMessage(message);
	CommitIfNeeded();
}

void PersistentConnectionInternalClient::ProcessMessage(const string& message)
{
	size_t commandMessageIdSeparator = message.find(' ');

	size_t messageIdTimestampSeparator = message.find(' ', commandMessageIdSeparator + 1);
	if(messageIdTimestampSeparator == string::npos)
		return;
	
	size_t timestampMessageSeparator = message.find(' ', messageIdTimestampSeparator + 1);
	if(timestampMessageSeparator == string::npos)
		return;

	string messageIdStr = message.substr(commandMessageIdSeparator + 1, messageIdTimestampSeparator - commandMessageIdSeparator);
	string timestampStr = message.substr(messageIdTimestampSeparator + 1, timestampMessageSeparator - messageIdTimestampSeparator);
	string payload = message.substr(timestampMessageSeparator + 1);

	PushedMessage pushedMessage;
	pushedMessage.MessageId = (int)atol(messageIdStr.c_str());
	pushedMessage.Timestamp = atol(timestampStr.c_str());
	pushedMessage.Payload = payload;

	_onMessageReceived(pushedMessage);
}

void PersistentConnectionInternalClient::CommitIfNeeded()
{
	if(_subscriptionType == PeekAndCommit)
	{
		_webSocketConnection->Send("commit");
	}
}

bool PersistentConnectionInternalClient::OnMessagePayloadReceived(const void* object, const string& message)
{
	PersistentConnectionInternalClient* pcic = (PersistentConnectionInternalClient*)object;
	if(message == "yo")
	{
		pcic->_webSocketConnection->Send("heartbeat");
		return true;
	}

	if(message.find("pushedmessage ") == 0)
	{
		pcic->HandlePushedMessage(message);
		return true;
	}

	return false;
}
}}

