#include "ManagementClient.h"
#include "../RestConnection.h"
#include "common.h"
#include "ArduinoJson.h"

CompanyManagementClient::CompanyManagementClient(RestConnection* restConnection)
{
	_restConnection = restConnection;
}

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

int CompanyManagementClient::Delete(const string& id)
{
	Response httpResponse =_restConnection->Delete("companies/" + id);

	if(httpResponse.Code != 204)
		return httpResponse.Code;

	return 0;
}

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

int CompanyManagementClient::UpdateIncomingTelemetryDataSinks(const string& id, const vector<TelemetryDataSinkParameters>& telemetryDataSinkParameters)
{
	string tdspJsonString = BuildTelmetryDataSinkParametersListJson(telemetryDataSinkParameters);

	Response httpResponse = _restConnection->Post("companies/" + id + "/incomingTelemetryDataSinks", "application/json", tdspJsonString);

	if(httpResponse.Code!= 204)
		return httpResponse.Code;

	return 0;
}

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

