#include "ManagementClient.h"
#include "../RestConnection.h"
#include "common.h"
#include "ArduinoJson.h"

namespace Thriot { namespace Management {

/**
Create a new instance of the company management client

@param restConnection Class providing REST function calls

*/
CompanyManagementClient::CompanyManagementClient(RestConnection* restConnection)
{
	_restConnection = restConnection;
}

/**
List all companies that the currently logged in user has permission to.
The function returns an empty list on error.

@return List of companies*/
vector<Small> CompanyManagementClient::List()
{
	vector<Small> companies;
	Response httpResponse = _restConnection->Get("companies");

	if(httpResponse.Code!= 200)
		return companies;

	DynamicJsonBuffer jsonBufferResponse;
	JsonArray& respJson = jsonBufferResponse.parseArray((char *)httpResponse.Body.c_str());

	for(JsonArray::iterator element = respJson.begin(); element!= respJson.end(); ++element)
	{
		Small small;
		small.Id = (*element)["Id"].asString();
		small.Name = (*element)["Name"].asString();
		companies.push_back(small);
	}

	return companies;
}

/**
Get a single company by it's unique identifier.
The function returns an uninitialized entity on error.

@param id Company unique identifier

@return A company entity */
Company CompanyManagementClient::Get(const string& id)
{
	Company company;

	Response httpResponse = _restConnection->Get(string("companies/") + id);

	if(httpResponse.Code!= 200)
		return company;

	DynamicJsonBuffer jsonBufferResponse;
	JsonObject& respJson = jsonBufferResponse.parseObject((char *)httpResponse.Body.c_str());
	
	company.Id = string(respJson["Id"].asString());
	company.Name = string(respJson["Name"].asString());

	ParseTelemetryDataSinkSettings(respJson, company.TelemetryDataSinkSettings);

	return company;
}

/**
Create a new company. The function returns an empty string on error.

@param company Company parameters

@return 32-characters long unique identifier of the company */
string CompanyManagementClient::Create(const Company& company)
{
	DynamicJsonBuffer jsonBufferRequest;
	JsonObject& createJson = jsonBufferRequest.createObject();

	createJson["Name"] = company.Name.c_str();

	char jsonBuffer[256];
	createJson.printTo(jsonBuffer, sizeof(jsonBuffer));

	Response httpResponse =_restConnection->Post("companies", "application/json",jsonBuffer);

	if(httpResponse.Code != 200)
		return "";

	string id = httpResponse.Body;
	StripQuotes(id);

	return id;
}

/**
Update a company. On error return the http status code otherwise 0.

@param company Company to update

@return status code */
int CompanyManagementClient::Update(const Company& company)
{
	DynamicJsonBuffer jsonBufferRequest;
	JsonObject& updateJson = jsonBufferRequest.createObject();

	updateJson["Id"] = company.Id.c_str();
	updateJson["Name"] = company.Name.c_str();

	char jsonBuffer[256];
	updateJson.printTo(jsonBuffer, sizeof(jsonBuffer));

	Response httpResponse =_restConnection->Put("companies", "application/json", jsonBuffer);

	if(httpResponse.Code != 204)
		return httpResponse.Code;

	return 0;
}

/**
Delete a company specified by id. On error return the http status code otherwise 0.

@param id Company specified by id to delete

@return status code*/
int CompanyManagementClient::Delete(const string& id)
{
	Response httpResponse =_restConnection->Delete("companies/" + id);

	if(httpResponse.Code != 204)
		return httpResponse.Code;

	return 0;
}

/**
List services that are set up under the company. On error the function returns an empty list.

@param id Company id

@return List of services*/
vector<Small> CompanyManagementClient::ListServices(const string& id)
{
	vector<Small> services;
	Response httpResponse = _restConnection->Get("companies/" + id + "/services");

	if(httpResponse.Code!= 200)
		return services;

	DynamicJsonBuffer jsonBufferResponse;
	JsonArray& respJson = jsonBufferResponse.parseArray((char *)httpResponse.Body.c_str());

	for(JsonArray::iterator element = respJson.begin(); element!= respJson.end(); ++element)
	{
		Small small;
		small.Id = (*element)["Id"].asString();
		small.Name = (*element)["Name"].asString();
		services.push_back(small);
	}

	return services;
}

/**
Updates the telemetry data sinks specified for a company.
On error the functions returns the http status code otherwise 0.

@param id Company identifier
@param telemetryDataSinkParameters Collection of parameterized telemetry data sinks

@return status code */
int CompanyManagementClient::UpdateIncomingTelemetryDataSinks(const string& id, const vector<TelemetryDataSinkParameters>& telemetryDataSinkParameters)
{
	string tdspJsonString = BuildTelmetryDataSinkParametersListJson(telemetryDataSinkParameters);

	Response httpResponse = _restConnection->Post("companies/" + id + "/incomingTelemetryDataSinks", "application/json", tdspJsonString);

	if(httpResponse.Code!= 204)
		return httpResponse.Code;

	return 0;
}

/**
List users who have access to the company.
On error an empty list will be returned.

@param id Id of the company

@return List of users */
vector<SmallUser> CompanyManagementClient::ListUsers(const string& id)
{
	vector<SmallUser> services;
	Response httpResponse = _restConnection->Get("companies/" + id + "/users");

	if(httpResponse.Code!= 200)
		return services;

	DynamicJsonBuffer jsonBufferResponse;
	JsonArray& respJson = jsonBufferResponse.parseArray((char *)httpResponse.Body.c_str());

	for(JsonArray::iterator element = respJson.begin(); element!= respJson.end(); ++element)
	{
		SmallUser small;
		small.Id = (*element)["Id"].asString();
		small.Name = (*element)["Name"].asString();
		services.push_back(small);
	}

	return services;
}

/**
Add users to the company.
Http status code on error, otherwise 0.

@param companyId Company identifier
@param userId User identifier

@return status code*/
int CompanyManagementClient::AddUser(const string& companyId, const string& userId)
{
	DynamicJsonBuffer jsonBufferRequest;
	JsonObject& updateJson = jsonBufferRequest.createObject();

	updateJson["CompanyId"] = companyId.c_str();
	updateJson["UserId"] = userId.c_str();

	char jsonBuffer[256];
	updateJson.printTo(jsonBuffer, sizeof(jsonBuffer));

	Response httpResponse =_restConnection->Post("companies/addUser", "application/json", jsonBuffer);

	if(httpResponse.Code != 204)
		return httpResponse.Code;

	return 0;
}
}}

