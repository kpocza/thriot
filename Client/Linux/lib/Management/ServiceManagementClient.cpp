#include "ManagementClient.h"
#include "../RestConnection.h"
#include "common.h"
#include "ArduinoJson.h"

namespace Thriot { namespace Management {

/**
Create a new instance of the service management client

@param restConnection Class providing REST function calls
*/
ServiceManagementClient::ServiceManagementClient(RestConnection* restConnection)
{
	_restConnection = restConnection;
}

/**
Get a single service by it's unique identifier.
The function returns an uninitialized entity on error.

@param id Service unique identifier

@return A service entity */
Service ServiceManagementClient::Get(const string& id)
{
	Service service;

	Response httpResponse = _restConnection->Get(string("services/") + id);

	if(httpResponse.Code!= 200)
		return service;

	DynamicJsonBuffer jsonBufferResponse;
	JsonObject& respJson = jsonBufferResponse.parseObject((char *)httpResponse.Body.c_str());
	
	service.Id = string(respJson["Id"].asString());
	service.Name = string(respJson["Name"].asString());
	service.ApiKey = string(respJson["ApiKey"].asString());
	service.CompanyId = string(respJson["CompanyId"].asString());

	ParseTelemetryDataSinkSettings(respJson, service.TelemetryDataSinkSettings);

	return service;
}

/**
Create a new service. The function returns an empty string on error.

@param service Service parameters

@return 32-characters long unique identifier of the service */
string ServiceManagementClient::Create(const Service& service)
{
	DynamicJsonBuffer jsonBufferRequest;
	JsonObject& createJson = jsonBufferRequest.createObject();

	createJson["Name"] = service.Name.c_str();
	createJson["CompanyId"] = service.CompanyId.c_str();

	char jsonBuffer[256];
	createJson.printTo(jsonBuffer, sizeof(jsonBuffer));

	Response httpResponse =_restConnection->Post("services", "application/json", jsonBuffer);

	if(httpResponse.Code != 200)
		return "";

	string id = httpResponse.Body;
	StripQuotes(id);

	return id;
}

/**
Update a service. On error return the http status code otherwise 0.

@param service Service to update

@return status code */
int ServiceManagementClient::Update(const Service& service)
{
	DynamicJsonBuffer jsonBufferRequest;
	JsonObject& updateJson = jsonBufferRequest.createObject();

	updateJson["Id"] = service.Id.c_str();
	updateJson["Name"] = service.Name.c_str();

	char jsonBuffer[256];
	updateJson.printTo(jsonBuffer, sizeof(jsonBuffer));

	Response httpResponse =_restConnection->Put("services", "application/json", jsonBuffer);

	if(httpResponse.Code != 204)
		return httpResponse.Code;

	return 0;
}

/**
Delete a service specified by id. On error return the http status code otherwise 0.

@param id Service specified by id to delete

@return status code*/
int ServiceManagementClient::Delete(const string& id)
{
	Response httpResponse =_restConnection->Delete("services/" + id);

	if(httpResponse.Code != 204)
		return httpResponse.Code;

	return 0;
}

/**
List networks that are set up under the service. On error the function returns an empty list.

@param id Service id

@return List of networks*/
vector<Small> ServiceManagementClient::ListNetworks(const string& id)
{
	vector<Small> networks;
	Response httpResponse = _restConnection->Get("services/" + id + "/networks");

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
Updates the telemetry data sinks specified for a service.
On error the functions returns the http status code otherwise 0.

@param id Service identifier
@param telemetryDataSinkParameters Collection of parameterized telemetry data sinks

@return status code */
int ServiceManagementClient::UpdateIncomingTelemetryDataSinks(const string& id, const vector<TelemetryDataSinkParameters>& telemetryDataSinkParameters)
{
	string tdspJsonString = BuildTelmetryDataSinkParametersListJson(telemetryDataSinkParameters);

	Response httpResponse = _restConnection->Post("services/" + id + "/incomingTelemetryDataSinks", "application/json", tdspJsonString);

	if(httpResponse.Code!= 204)
		return httpResponse.Code;

	return 0;
}
}}

