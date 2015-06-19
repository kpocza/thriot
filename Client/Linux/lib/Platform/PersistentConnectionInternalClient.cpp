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

PlatformOperationResult PersistentConnectionInternalClient::Heartbeat()
{
	if(!_isLoggedIn)
		return LoginRequired;

	if(_webSocketConnection->IsDisconnected())
		return Disconnected;

	_webSocketConnection->Send("heartbeat");

	string response = _webSocketConnection->WaitResponse(10);

	if(!response.empty())
	{
		if(response == "yo")
			return Ok;

		return HeartbeatInvalid;
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
	// pushedmessage <messageid> <timestamp> <senderdeviceid> <payload>
	size_t afterCommandSeparator = message.find(' ');

	size_t afterMessageIdSeparator = message.find(' ', afterCommandSeparator + 1);
	if(afterMessageIdSeparator == string::npos)
		return;
	
	size_t afterTimestampSeparator = message.find(' ', afterMessageIdSeparator + 1);
	if(afterTimestampSeparator == string::npos)
		return;

	size_t afterSenderDeviceIdSeparator = message.find(' ', afterTimestampSeparator + 1);
	if(afterSenderDeviceIdSeparator == string::npos)
		return;

	string messageIdStr = message.substr(afterCommandSeparator + 1, afterMessageIdSeparator - afterCommandSeparator - 1);
	string timestampStr = message.substr(afterMessageIdSeparator + 1, afterTimestampSeparator - afterMessageIdSeparator - 1);
	string senderDeviceIdStr = message.substr(afterTimestampSeparator + 1, afterSenderDeviceIdSeparator - afterTimestampSeparator - 1);
	string payload = message.substr(afterSenderDeviceIdSeparator + 1);

	PushedMessage pushedMessage;
	pushedMessage.MessageId = (int)atol(messageIdStr.c_str());
	pushedMessage.Timestamp = atol(timestampStr.c_str());
	pushedMessage.SenderDeviceId = senderDeviceIdStr;
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

	if(message.find("pushedmessage ") == 0)
	{
		pcic->HandlePushedMessage(message);
		return true;
	}

	return false;
}
}}

