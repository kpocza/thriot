#include "ManagementClient.h"
#include "../RestConnection.h"
#include "common.h"
#include "ArduinoJson.h"

namespace Thriot { namespace Management {

ServiceManagementClient::ServiceManagementClient(RestConnection* restConnection)
{
	_restConnection = restConnection;
}

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

int ServiceManagementClient::Delete(const string& id)
{
	Response httpResponse =_restConnection->Delete("services/" + id);

	if(httpResponse.Code != 204)
		return httpResponse.Code;

	return 0;
}

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

int ServiceManagementClient::UpdateIncomingTelemetryDataSinks(const string& id, const vector<TelemetryDataSinkParameters>& telemetryDataSinkParameters)
{
	string tdspJsonString = BuildTelmetryDataSinkParametersListJson(telemetryDataSinkParameters);

	Response httpResponse = _restConnection->Post("services/" + id + "/incomingTelemetryDataSinks", "application/json", tdspJsonString);

	if(httpResponse.Code!= 204)
		return httpResponse.Code;

	return 0;
}
}}

