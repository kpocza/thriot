#pragma once

#include <string>
#include <map>
#include <vector>

using namespace std;

namespace Thriot { 
class RestConnection;

namespace Management {

struct LoginInfo
{
	string Email;
	string Password;
};

struct RegisterInfo : LoginInfo
{
	string Name;
};

struct User
{
	string Id;
	string Name;
	string Email;
};

struct TelemetryDataSinkParameters
{
	string SinkName;
	map<string, string> Parameters;
};

struct TelemetryDataSinkSettingsType
{
	vector<TelemetryDataSinkParameters> Incoming;
};

struct TelemetryDataSinkMetadata
{
	string Name;
	string Description;
	vector<string> ParametersToInput;
};

struct TelemetryDataSinksMetadata
{
	vector<TelemetryDataSinkMetadata> Incoming;
};

struct Company
{
	string Id;
	string Name;
	TelemetryDataSinkSettingsType TelemetryDataSinkSettings;
};

struct Service
{
	string Id;
	string Name;
	string CompanyId;
	string ApiKey;
	TelemetryDataSinkSettingsType TelemetryDataSinkSettings;
};

struct Network
{
	string Id;
	string Name;
	string NetworkKey;
	string ParentNetworkId;
	string ServiceId;
	string CompanyId;
	TelemetryDataSinkSettingsType TelemetryDataSinkSettings;
};

struct Device
{
	string Id;
	string Name;
	string DeviceKey;
	string NetworkId;
	string ServiceId;
	string CompanyId;
};

struct Small
{
	string Id;
	string Name;
};


struct SmallUser
{
	string Id;
	string Name;
	string Email;
};

class UserManagementClient
{
	private:
		RestConnection* _restConnection;

	public:
		UserManagementClient(RestConnection* restConnection);
		int Register(const RegisterInfo& reg);
		int Login(const LoginInfo& login);
		void Logoff();
		bool IsLoggedIn();
		User Get();
		User FindUser(const string& email);
};

class TelemetryDataSinksMetadataClient
{
	private:
		RestConnection* _restConnection;

	public:
		TelemetryDataSinksMetadataClient(RestConnection* restConnection);
		TelemetryDataSinksMetadata Get();
};

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

