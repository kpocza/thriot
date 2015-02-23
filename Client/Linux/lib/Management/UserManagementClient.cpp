#include <stdio.h>
#include <iostream>
#include <string.h>
#include "ManagementClient.h"
#include "../RestConnection.h"
#include "ArduinoJson.h"
#include "common.h"

using namespace std;

UserManagementClient::UserManagementClient(RestConnection* restConnection) 
{
	_restConnection = restConnection;
}

int UserManagementClient::Register(const RegisterInfo& reg)
{
	DynamicJsonBuffer jsonBufferRequest;
	JsonObject& regJson = jsonBufferRequest.createObject();

	regJson["Name"] = reg.Name.c_str();
	regJson["Email"] = reg.Email.c_str();
	regJson["Password"] = reg.Password.c_str();

	char jsonBuffer[256];
	regJson.printTo(jsonBuffer, sizeof(jsonBuffer));

	Response httpResponse =_restConnection->Post("users/register", "application/json", jsonBuffer);

	if(httpResponse.Code != 200)
		return httpResponse.Code;

	DynamicJsonBuffer jsonBufferResponse;
	JsonObject& respJson = jsonBufferResponse.parseObject((char *)httpResponse.Body.c_str());
	
	bool needsActivation = (bool)respJson["NeedsActivation"];
	
	if(needsActivation)
		return -1;

	const char* authToken = respJson["AuthToken"].asString();

	_restConnection->SetAuthToken(string(authToken));

	return 0;	
}

int UserManagementClient::Login(const LoginInfo& login)
{
	DynamicJsonBuffer jsonBufferRequest;

	JsonObject& regJson = jsonBufferRequest.createObject();

	regJson["Email"] = login.Email.c_str();
	regJson["Password"] = login.Password.c_str();

	char jsonBuffer[256];
	regJson.printTo(jsonBuffer, sizeof(jsonBuffer));

	Response httpResponse =_restConnection->Post("users/login", "application/json", string(jsonBuffer));

	if(httpResponse.Code != 200)
		return httpResponse.Code;

	string authToken = httpResponse.Body;

	StripQuotes(authToken);

	_restConnection->SetAuthToken(string(authToken));

	return 0;	
}

void UserManagementClient::Logoff()
{
	_restConnection->ClearAuthToken();
}

bool UserManagementClient::IsLoggedIn()
{
	return _restConnection->IsAuthenticated();
}

User UserManagementClient::Get()
{
	Response httpResponse =_restConnection->Get("users/me");

	User user;
	
	if(httpResponse.Code != 200)
		return user;

	DynamicJsonBuffer jsonBufferResponse;
	JsonObject& respJson = jsonBufferResponse.parseObject((char *)httpResponse.Body.c_str());
	
	user.Id = string(respJson["Id"].asString());
	user.Name = string(respJson["Name"].asString());
	user.Email = string(respJson["Email"].asString());

	return user;
}

User UserManagementClient::FindUser(const string& email)
{
	Response httpResponse =_restConnection->Get("users/byemail/" + UrlEncode(email) + "/");

	User user;
	
	if(httpResponse.Code != 200)
		return user;

	DynamicJsonBuffer jsonBufferResponse;
	JsonObject& respJson = jsonBufferResponse.parseObject((char *)httpResponse.Body.c_str());
	
	user.Id = string(respJson["Id"].asString());
	user.Name = string(respJson["Name"].asString());
	user.Email = string(respJson["Email"].asString());

	return user;
}

