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
	_isLoggedIn = false;
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

	_isLoggedIn = true;

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

	if(httpResponse.Code != 204)
		return httpResponse.Code;

	_isLoggedIn = true;

	return 0;
}

/**
Activate the user.
Generaly this method wouldn't be called from the client sinde the activation link is embedded into the
email intended to force user activation.

@param activate Activation parameters (userid and activation code)

@return operation status. 0 if Ok*/
int UserManagementClient::Activate(const ActivateInfo& activate)
{
	Response httpResponse =_restConnection->Get("users/activate/" + activate.UserId + "/" + activate.ActivationCode);

	if(httpResponse.Code != 204)
		return httpResponse.Code;

	_isLoggedIn = true;

	return 0;
}

/**
Resend activation email
When the activation email didn't arrive this method can be used to resend it.

@param email Email address to resend activation email to

@return operation status 0 if Ok */
int UserManagementClient::ResendActivationEmail(const string& email)
{
	DynamicJsonBuffer jsonBufferRequest;

	JsonObject& regJson = jsonBufferRequest.createObject();

	regJson["Email"] = email.c_str();

	char jsonBuffer[256];
	regJson.printTo(jsonBuffer, sizeof(jsonBuffer));

	Response httpResponse =_restConnection->Post("users/resendActivationEmail", "application/json", string(jsonBuffer));

	if(httpResponse.Code != 204)
		return httpResponse.Code;

	return 0;
}

/**
Send reset forgot password email
When the user forgot the password send an email with a link that can be used to reset the password.

@param email Email address to sent forgot password email

@return operation status 0 if Ok */
int UserManagementClient::SendForgotPasswordEmail(const string& email)
{
	DynamicJsonBuffer jsonBufferRequest;

	JsonObject& regJson = jsonBufferRequest.createObject();

	regJson["Email"] = email.c_str();

	char jsonBuffer[256];
	regJson.printTo(jsonBuffer, sizeof(jsonBuffer));

	Response httpResponse =_restConnection->Post("users/sendForgotPasswordEmail", "application/json", string(jsonBuffer));

	if(httpResponse.Code != 204)
		return httpResponse.Code;

	return 0;
}

/**
Reset password. 
This method generaly employs has the parameters that the SendForgotPasswordEmail contained

@param resetPassword Class containing reset password info

@return operation status 0 if Ok */
int UserManagementClient::ResetPassword(const ResetPasswordInfo& resetPassword)
{
	DynamicJsonBuffer jsonBufferRequest;

	JsonObject& regJson = jsonBufferRequest.createObject();

	regJson["UserId"] = resetPassword.UserId.c_str();
	regJson["ConfirmationCode"] = resetPassword.ConfirmationCode.c_str();
	regJson["Password"] = resetPassword.Password.c_str();

	char jsonBuffer[256];
	regJson.printTo(jsonBuffer, sizeof(jsonBuffer));

	Response httpResponse =_restConnection->Post("users/resetPassword", "application/json", string(jsonBuffer));

	if(httpResponse.Code != 204)
		return httpResponse.Code;

	return 0;
}

/**
Change password. 
Change password of the already logged in user to use a different password in the future.

@param changePassword Class containing change password info

@return operation status 0 if Ok */
int UserManagementClient::ChangePassword(const ChangePasswordInfo& changePassword)
{
	DynamicJsonBuffer jsonBufferRequest;

	JsonObject& regJson = jsonBufferRequest.createObject();

	regJson["CurrentPassword"] = changePassword.CurrentPassword.c_str();
	regJson["NewPassword"] = changePassword.NewPassword.c_str();

	char jsonBuffer[256];
	regJson.printTo(jsonBuffer, sizeof(jsonBuffer));

	Response httpResponse =_restConnection->Post("users/changePassword", "application/json", string(jsonBuffer));

	if(httpResponse.Code != 204)
		return httpResponse.Code;

	return 0;
}

/** Logoff the currently logged-in user */
void UserManagementClient::Logoff()
{
	_restConnection->Post("users/logoff", "application/json", "");

	_isLoggedIn = false;
}

/**
Determines whether the user is logged in

@return user login status*/
bool UserManagementClient::IsLoggedIn()
{
	return _isLoggedIn;
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

