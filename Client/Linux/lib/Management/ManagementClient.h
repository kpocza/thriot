#pragma once

#include <string>
#include <map>
#include <vector>

using namespace std;

namespace Thriot { 
class RestConnection;

namespace Management {

/** DTO class used to store login information */
struct LoginInfo
{
	/** Email address */
	string Email;
	/** Plain-text password */
	string Password;
};

/** DTO used for registration */
struct RegisterInfo : LoginInfo
{
	/** Full name of the user */
	string Name;
};

/** DTO used when a single or multiple User entities are returned */
struct User
{
	/** 32-character long unique Id */
	string Id;
	/** Full name */
	string Name;
	/** Email address */
	string Email;
};

/** DTO used to activate the user */
struct ActivateInfo
{
	/** Unique user identifier */
	string UserId;
	/** Random activation code */
	string ActivationCode;
};

/** DTO used to reset user's password */
struct ResetPasswordInfo
{
	/** Unique user identifier */
	string UserId;
	/** Random confirmation code */
	string ConfirmationCode;
	/** Password */
	string Password;
};

/** DTO for changing password */
struct ChangePasswordInfo
{
	/** Current password of the user */
	string CurrentPassword;
	/** New password for future use */
	string NewPassword;
};

/** A parameterized telemetry data sink entity */
struct TelemetryDataSinkParameters
{
	/** Telemetry data sink name */
	string SinkName;
	/** Filled in parameters of the telemetry data sink */
	map<string, string> Parameters;
};

/** Types that encapsulates multiple parameterized telemetry data sinks */
struct TelemetryDataSinkSettingsType
{
	/** Multiple parameterized telemetry data sinks */
	vector<TelemetryDataSinkParameters> Incoming;
};

/** Telemetry data sink metadata used to provide information about the telemetry data sink and required parameters */
struct TelemetryDataSinkMetadata
{
	/** Name of the telemetry data sink */
	string Name;
	/** Description */
	string Description;
	/** Parameters identified by their names to fill */
	vector<string> ParametersToInput;
};

/** Class that encapsulates multiple telemetry data sink metadata entities */
struct TelemetryDataSinksMetadata
{
	/** List of telemetry data sink metadata entities */
	vector<TelemetryDataSinkMetadata> Incoming;
};

/** DTO entity for transferring company information */
struct Company
{
	/** 32-characters long unique identifier */
	string Id;
	/** Name of the comapany */
	string Name;
	/** Parameterized telemetry data sinks */
	TelemetryDataSinkSettingsType TelemetryDataSinkSettings;
};

/** DTO entity for transferring service information */
struct Service
{
	/** 32-characters long unique identifier */
	string Id;
	/** Name of the service */
	string Name;
	/** Enclosing company id */
	string CompanyId;
	/** API key that all devices attached to this service can use for authentication */
	string ApiKey;
	/** Parameterized telemetry data sinks */
	TelemetryDataSinkSettingsType TelemetryDataSinkSettings;
};

/** DTO entity for transferring network information */
struct Network
{
	/** 32-characters long unique identifier */
	string Id;
	/** Name of the network */
	string Name;
	/** API key that all devices attached to this network can use for authentication */
	string NetworkKey;
	/** If the network has a parent network then the id of the parent network otherwise empty string */
	string ParentNetworkId;
	/** Id of the enclosing service */
	string ServiceId;
	/** Id of the enclosing company */
	string CompanyId;
	/** Parameterized telemetry data sinks */
	TelemetryDataSinkSettingsType TelemetryDataSinkSettings;
};

/** DTO entity for transferring device information */
struct Device
{
	/** 32-characters long unique identifier */
	string Id;
	/** Name of the device */
	string Name;
	/** API Key that can be used for authenticating this device */
	string DeviceKey;
	/** Id of the enclosing network */
	string NetworkId;
	/** Id of the enclosing service */
	string ServiceId;
	/** Id of the enclosing company */
	string CompanyId;
};

/** DTO entity for transferring basic entity information */
struct Small
{
	/** 32-characters long unique identifier */
	string Id;
	/** Name of the basic entity */
	string Name;
};


/** DTO entity for transferring basic user information */
struct SmallUser
{
	/** 32-characters long unique identifier */
	string Id;
	/** Name of the basic user entity */
	string Name;
	/** Email address */
	string Email;
};

/** Class that is used to manage users and profile login/logoff functionality */
class UserManagementClient
{
	private:
		RestConnection* _restConnection;
		bool _isLoggedIn;

	public:
		UserManagementClient(RestConnection* restConnection);
		int Register(const RegisterInfo& reg);
		int Activate(const ActivateInfo& activate);
		int ResendActivationEmail(const string& email);
		int SendForgotPasswordEmail(const string& email);
		int ResetPassword(const ResetPasswordInfo& resetPassword);
		int ChangePassword(const ChangePasswordInfo& changePassword);
		int Login(const LoginInfo& login);
		void Logoff();
		bool IsLoggedIn();
		User Get();
		User FindUser(const string& email);
};

/** Query telemetry metadata */
class TelemetryDataSinksMetadataClient
{
	private:
		RestConnection* _restConnection;

	public:
		TelemetryDataSinksMetadataClient(RestConnection* restConnection);
		TelemetryDataSinksMetadata Get();
};

/** Manage companies */
class CompanyManagementClient
{
	private:
		RestConnection* _restConnection;

	public:
		CompanyManagementClient(RestConnection* restConnection);
		vector<Small> List();
		Company Get(const string& id);
		string Create(const Company& company);
		int Update(const Company& company);
		int Delete(const string& id);
		vector<Small> ListServices(const string& id);
		int UpdateIncomingTelemetryDataSinks(const string& id, const vector<TelemetryDataSinkParameters>& telemetryDataSinkParameters);
		vector<SmallUser> ListUsers(const string& id);
		int AddUser(const string& companyId, const string& userId);
};

/** Manage services */
class ServiceManagementClient
{
	private:
		RestConnection* _restConnection;

	public:
		ServiceManagementClient(RestConnection* restConnection);
		Service Get(const string& id);
		string Create(const Service& service);
		int Update(const Service& service);
		int Delete(const string& id);
		vector<Small> ListNetworks(const string& id);
		int UpdateIncomingTelemetryDataSinks(const string& id, const vector<TelemetryDataSinkParameters>& telemetryDataSinkParameters);
};

/** Manage networks */
class NetworkManagementClient
{
	private:
		RestConnection* _restConnection;

	public:
		NetworkManagementClient(RestConnection* restConnection);
		Network Get(const string& id);
		string Create(const Network& network);
		int Update(const Network& network);
		int Delete(const string& id);
		vector<Small> ListNetworks(const string& id);
		vector<Small> ListDevices(const string& id);
		int UpdateIncomingTelemetryDataSinks(const string& id, const vector<TelemetryDataSinkParameters>& telemetryDataSinkParameters);
};

/** Manage devices */
class DeviceManagementClient
{
	private:
		RestConnection* _restConnection;

	public:
		DeviceManagementClient(RestConnection* restConnection);
		Device Get(const string& id);
		string Create(const Device& device);
		int Update(const Device& device);
		int Delete(const string& id);
};

/** Encapsulate all management classes */
class ManagementClient
{
	private:
		RestConnection* _restConnection;
		UserManagementClient* _userManagementClient;
		CompanyManagementClient* _companyManagementClient;
		ServiceManagementClient* _serviceManagementClient;
		NetworkManagementClient* _networkManagementClient;
		DeviceManagementClient* _deviceManagementClient;
		TelemetryDataSinksMetadataClient* _telemetryDataSinksMetadataClient;

	public:
		ManagementClient(const string& baseUrl);
		~ManagementClient();

		UserManagementClient* User();
		CompanyManagementClient* Company();
		ServiceManagementClient* Service();
		NetworkManagementClient* Network();
		DeviceManagementClient* Device();
		TelemetryDataSinksMetadataClient* TelemetryDataSinksMetadata();
};
}}

