#include "PlatformClient.h"
#include "ArduinoJson.h"
#include "../RestConnection.h"
#include "../base64.h"

OcassionalConnectionClient::OcassionalConnectionClient(const string& baseUrl, const string& deviceId, const string& apiKey)
{
	_restConnection = new RestConnection(baseUrl);
	_restConnection->AddRequestHeader("X-DeviceId", deviceId);
	_restConnection->AddRequestHeader("X-ApiKey", apiKey);
}

// In this class:
// we are not handling character codes above or eq 0x80
// TODO: fix to support at least latin1 chars

int OcassionalConnectionClient::RecordTelemetryData(const string& message)
{
	Response httpResponse = _restConnection->Post("telemetry", "application/json", message);

	if(httpResponse.Code!= 204)
		return httpResponse.Code;

	return 0;
}

int OcassionalConnectionClient::SendMessageTo(const string& deviceId, const string& message)
{
	string payload = "\"" + base64_encode((unsigned char *)message.c_str(), message.size()) + "\"";

	Response httpResponse = _restConnection->Post("messages/sendto/" + deviceId, "application/json", payload);

	if(httpResponse.Code!= 204)
		return httpResponse.Code;

	return 0;
}

PushedMessage OcassionalConnectionClient::ReceiveAndForgetMessage()
{
	return Receive("messages/forget");
}

PushedMessage OcassionalConnectionClient::PeekMessage()
{
	return Receive("messages/peek");
}

int OcassionalConnectionClient::CommitMessage()
{
	Response httpResponse = _restConnection->Post("messages/commit", "application/json", "");

	if(httpResponse.Code!= 204)
		return httpResponse.Code;

	return 0;
}

PushedMessage OcassionalConnectionClient::Receive(const string& url)
{
	PushedMessage pushedMessage;

	Response httpResponse = _restConnection->Get(url);

	if(httpResponse.Body == "null")
	{
		pushedMessage.MessageId = -1;
		return pushedMessage;
	}

	if(httpResponse.Code!= 200)
	{
		pushedMessage.MessageId = -httpResponse.Code;
		return pushedMessage;
	}

	DynamicJsonBuffer jsonBufferResponse;
	JsonObject& respJson = jsonBufferResponse.parseObject((char *)httpResponse.Body.c_str());
	
	pushedMessage.MessageId = (int32_t)respJson["MessageId"];
	pushedMessage.Payload = string(respJson["Payload"].asString());
	pushedMessage.Timestamp = (long unsigned int)respJson["Timestamp"];

	return pushedMessage;
}
