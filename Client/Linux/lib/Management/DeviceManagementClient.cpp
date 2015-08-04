#include "ManagementClient.h"
#include "../RestConnection.h"
#include "common.h"
#include "ArduinoJson.h"

namespace Thriot { namespace Management {

/**
Create a new instance of the device management client

@param restConnection Class providing REST function calls
*/
DeviceManagementClient::DeviceManagementClient(RestConnection* restConnection)
{
	_restConnection = restConnection;
}

/**
Get a single device by it's unique identifier.
The function returns an uninitialized entity on error.

@param id Device unique identifier

@return A device entity */
Device DeviceManagementClient::Get(const string& id)
{
	Device device;

	Response httpResponse = _restConnection->Get(string("devices/") + id);

	if(httpResponse.Code!= 200)
		return device;

	DynamicJsonBuffer jsonBufferResponse;
	JsonObject& respJson = jsonBufferResponse.parseObject((char *)httpResponse.Body.c_str());
	
	device.Id = string(respJson["Id"].asString());
	device.Name = string(respJson["Name"].asString());
	device.CompanyId = string(respJson["CompanyId"].asString());
	device.ServiceId = string(respJson["ServiceId"].asString());
	device.NetworkId = string(respJson["NetworkId"].asString());
	device.DeviceKey = string(respJson["DeviceKey"].asString());

	return device;
}

/**
Create a new device. The function returns an empty string on error.

@param device Device parameters

@return 32-characters long unique identifier of the device */
string DeviceManagementClient::Create(const Device& device)
{
	DynamicJsonBuffer jsonBufferRequest;
	JsonObject& createJson = jsonBufferRequest.createObject();

	createJson["Name"] = device.Name.c_str();
	createJson["CompanyId"] = device.CompanyId.c_str();
	createJson["ServiceId"] = device.ServiceId.c_str();
	createJson["NetworkId"] = device.NetworkId.c_str();

	char jsonBuffer[256];
	createJson.printTo(jsonBuffer, sizeof(jsonBuffer));

	Response httpResponse =_restConnection->Post("devices", "application/json", jsonBuffer);

	if(httpResponse.Code != 200)
		return "";

	string id = httpResponse.Body;
	StripQuotes(id);

	return id;
}

/**
Update a device. On error return the http status code otherwise 0.

@param device Device to update

@return status code */
int DeviceManagementClient::Update(const Device& device)
{
	DynamicJsonBuffer jsonBufferRequest;
	JsonObject& updateJson = jsonBufferRequest.createObject();

	updateJson["Id"] = device.Id.c_str();
	updateJson["Name"] = device.Name.c_str();

	char jsonBuffer[256];
	updateJson.printTo(jsonBuffer, sizeof(jsonBuffer));

	Response httpResponse =_restConnection->Put("devices", "application/json", jsonBuffer);

	if(httpResponse.Code != 200)
		return httpResponse.Code;

	return 0;
}

/**
Delete a device specified by id. On error return the http status code otherwise 0.

@param id Device specified by id to delete

@return status code*/
int DeviceManagementClient::Delete(const string& id)
{
	Response httpResponse =_restConnection->Delete("devices/" + id);

	if(httpResponse.Code != 200)
		return httpResponse.Code;

	return 0;
}
}}

