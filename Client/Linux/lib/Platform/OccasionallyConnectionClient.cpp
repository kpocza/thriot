#include "PlatformClient.h"
#include "ArduinoJson.h"
#include "../RestConnection.h"
#include "../base64.h"

namespace Thriot { namespace Platform {

/**
Create a new instance of the occasionally connection client

@param baseUrl Base Platform API url
@param deviceId Unique 32-characters long id
@param apiKey device key, enclosing network key or session key

*/
OccasionallyConnectionClient::OccasionallyConnectionClient(const string& baseUrl, const string& deviceId, const string& apiKey)
{
	_restConnection = new RestConnection(baseUrl);
	_restConnection->AddRequestHeader("X-DeviceId", deviceId);
	_restConnection->AddRequestHeader("X-ApiKey", apiKey);
}

// In this class:
// we are not handling character codes above or eq 0x80
// TODO: fix to support at least latin1 chars

/**
Record telemetry data. Send telemetry message to the service.
On error returns the http status code, otherwise 0.

@param message Telemetry data to be recorded. Maximum 1024 characters.

@return Status code (0 if everything went well) */
int OccasionallyConnectionClient::RecordTelemetryData(const string& message)
{
	Response httpResponse = _restConnection->Post("telemetry", "application/json", message);

	if(httpResponse.Code!= 200)
		return httpResponse.Code;

	return 0;
}

/**
Send message to an other device from the actually signed in device.
On error returns the http status code, otherwise 0.

@param deviceId Target device id
@param message Message to be sent (maximum 512 charactres)

@return Status code*/
int OccasionallyConnectionClient::SendMessageTo(const string& deviceId, const string& message)
{
	string payload = "\"" + base64_encode((unsigned char *)message.c_str(), message.size()) + "\"";

	Response httpResponse = _restConnection->Post("messages/sendto/" + deviceId, "application/json", payload);

	if(httpResponse.Code!= 204)
		return httpResponse.Code;

	return 0;
}

/**
Receive message and forget it on the service side.
If an error occures the message cannot be rereceived. This is QoS 0-level reliability from the receiver's side.
On error returns an uninitialized message.

@return Message instance*/
PushedMessage OccasionallyConnectionClient::ReceiveAndForgetMessage()
{
	return Receive("messages/forget");
}

/**
Receive message and keep it on the service side. A CommitMessage call should be issued to remove it from the service side.
If an error occures the message can be rereceived. This is QoS 1-level reliability from the receiver's side.
On error returns an uninitialized message.

@return */
PushedMessage OccasionallyConnectionClient::PeekMessage()
{
	return Receive("messages/peek");
}

/**
Removed the Message received by the PeekMessage call.
On error returns the http status code, otherwise 0.

@return */
int OccasionallyConnectionClient::CommitMessage()
{
	Response httpResponse = _restConnection->Post("messages/commit", "application/json", "");

	if(httpResponse.Code!= 204)
		return httpResponse.Code;

	return 0;
}

PushedMessage OccasionallyConnectionClient::Receive(const string& url)
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
	pushedMessage.SenderDeviceId = string(respJson["SenderDeviceId"].asString());
	pushedMessage.Payload = string(respJson["Payload"].asString());
	pushedMessage.Timestamp = (long unsigned int)respJson["Timestamp"];

	return pushedMessage;
}
}}

