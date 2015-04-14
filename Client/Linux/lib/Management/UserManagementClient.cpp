#include <stdio.h>
#include <iostream>
#include <string.h>
#include "ManagementClient.h"
#include "../RestConnection.h"
#include "ArduinoJson.h"
#include "common.h"

using namespace std;

namespace Thriot { namespace Management {

/**
Initializes a new instance of the user management client

@param restConnection Wrapper class for the rest API calls
*/
UserManagementClient::UserManagementClient(RestConnection* restConnection) 
{
	_restConnection = restConnection;
}

/**
Register a new user. 
Return 0 if there was no error, -1 if email activation is needed, HTTP status code on other error.
On successfull registration all subsequent operations will be executed in the name of the registered user.

@param reg Registration parameters

@return operation status. 0 if Ok.*/
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

/**
Login a new user.
On successfull login all subsequent operations will be executed in the name of the logged in user.
If everything was fine 0 is returned otherwise a http status code.

@param login Login user email and password

@return operation status. 0 if Ok*/
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

/** Logoff the currently logged-in user */
void UserManagementClient::Logoff()
{
	_restConnection->ClearAuthToken();
}


/**
Determines whether the user is logged in

@return user login status*/
bool UserManagementClient::IsLoggedIn()
{
	return _restConnection->IsAuthenticated();
}

/**
Get the currently logged in user.
If the operation is successfull a valid user object is returned otherwise an uninitialized one.

@return User object*/
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

/**
Find user by email address.
If the operation is successfull a valid user object is returned otherwise an uninitialized one.

@param email email address of the user we are looking for 

@return User object*/
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
}}

