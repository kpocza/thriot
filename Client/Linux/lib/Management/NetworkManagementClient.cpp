#include "ManagementClient.h"
#include "../RestConnection.h"
#include "common.h"
#include "ArduinoJson.h"

namespace Thriot { namespace Management {

/**
Create a new instance of the network management client

@param restConnection Class providing REST function calls
*/
NetworkManagementClient::NetworkManagementClient(RestConnection* restConnection)
{
	_restConnection = restConnection;
}

/**
Get a single network by it's unique identifier.
The function returns an uninitialized entity on error.

@param id Network unique identifier

@return A network entity */
Network NetworkManagementClient::Get(const string& id)
{
	Network network;

	Response httpResponse = _restConnection->Get(string("networks/") + id);

	if(httpResponse.Code!= 200)
		return network;
	
	DynamicJsonBuffer jsonBufferResponse;
	JsonObject& respJson = jsonBufferResponse.parseObject((char *)httpResponse.Body.c_str());
	
	network.Id = string(respJson["Id"].asString());
	network.Name = string(respJson["Name"].asString());
	network.CompanyId = string(respJson["CompanyId"].asString());
	network.ServiceId = string(respJson["ServiceId"].asString());
	network.NetworkKey = string(respJson["NetworkKey"].asString());

	if(respJson.containsKey("ParentNetworkId") && (bool)(respJson["ParentNetworkId"].asString() != NULL))
		network.ParentNetworkId = string(respJson["ParentNetworkId"].asString());

	ParseTelemetryDataSinkSettings(respJson, network.TelemetryDataSinkSettings);

	return network;
}

/**
Create a new network. The function returns an empty string on error.

@param network Network parameters

@return 32-characters long unique identifier of the network */
string NetworkManagementClient::Create(const Network& network)
{
	DynamicJsonBuffer jsonBufferRequest;
	JsonObject& createJson = jsonBufferRequest.createObject();

	createJson["Name"] = network.Name.c_str();
	createJson["CompanyId"] = network.CompanyId.c_str();
	createJson["ServiceId"] = network.ServiceId.c_str();
	if(!network.ParentNetworkId.empty())
		createJson["ParentNetworkId"] = network.ParentNetworkId.c_str();

	char jsonBuffer[256];
	createJson.printTo(jsonBuffer, sizeof(jsonBuffer));

	Response httpResponse =_restConnection->Post("networks", "application/json", jsonBuffer);

	if(httpResponse.Code != 200)
		return "";

	string id = httpResponse.Body;
	StripQuotes(id);

	return id;
}

/**
Update a network. On error return the http status code otherwise 0.

@param network Network to update

@return status code */
int NetworkManagementClient::Update(const Network& network)
{
	DynamicJsonBuffer jsonBufferRequest;
	JsonObject& updateJson = jsonBufferRequest.createObject();

	updateJson["Id"] = network.Id.c_str();
	updateJson["Name"] = network.Name.c_str();

	char jsonBuffer[256];
	updateJson.printTo(jsonBuffer, sizeof(jsonBuffer));

	Response httpResponse =_restConnection->Put("networks", "application/json", jsonBuffer);

	if(httpResponse.Code != 204)
		return httpResponse.Code;

	return 0;
}

/**
Delete a network specified by id. On error return the http status code otherwise 0.

@param id Network specified by id to delete

@return status code*/
int NetworkManagementClient::Delete(const string& id)
{
	Response httpResponse =_restConnection->Delete("networks/" + id);

	if(httpResponse.Code != 204)
		return httpResponse.Code;

	return 0;
}

/**
List networks that are set up under the parent network. On error the function returns an empty list.

@param id Parent network id

@return List of networks*/
vector<Small> NetworkManagementClient::ListNetworks(const string& id)
{
	vector<Small> networks;
	Response httpResponse = _restConnection->Get("networks/" + id + "/networks");

	if(httpResponse.Code!= 200)
		return networks;

	DynamicJsonBuffer jsonBufferResponse;
	JsonArray& respJson = jsonBufferResponse.parseArray((char *)httpResponse.Body.c_str());

	for(JsonArray::iterator element = respJson.begin(); element!= respJson.end(); ++element)
	{
		Small small;
		small.Id = (*element)["Id"].asString();
		small.Name = (*element)["Name"].asString();
		networks.push_back(small);
	}

	return networks;
}

/**
List devices that are set up under the network. On error the function returns an empty list.

@param id Network id

@return List of devices*/
vector<Small> NetworkManagementClient::ListDevices(const string& id)
{
	vector<Small> devices;
	Response httpResponse = _restConnection->Get("networks/" + id + "/devices");

	if(httpResponse.Code!= 200)
		return devices;

	DynamicJsonBuffer jsonBufferResponse;
	JsonArray& respJson = jsonBufferResponse.parseArray((char *)httpResponse.Body.c_str());

	for(JsonArray::iterator element = respJson.begin(); element!= respJson.end(); ++element)
	{
		Small small;
		small.Id = (*element)["Id"].asString();
		small.Name = (*element)["Name"].asString();
		devices.push_back(small);
	}

	return devices;
}

/**
Updates the telemetry data sinks specified for a network.
On error the functions returns the http status code otherwise 0.

@param id Network identifier
@param telemetryDataSinkParameters Collection of parameterized telemetry data sinks

@return status code */
int NetworkManagementClient::UpdateIncomingTelemetryDataSinks(const string& id, const vector<TelemetryDataSinkParameters>& telemetryDataSinkParameters)
{
	string tdspJsonString = BuildTelmetryDataSinkParametersListJson(telemetryDataSinkParameters);

	Response httpResponse = _restConnection->Post("networks/" + id + "/incomingTelemetryDataSinks", "application/json", tdspJsonString);

	if(httpResponse.Code!= 204)
		return httpResponse.Code;

	return 0;
}
}}

