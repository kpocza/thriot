#include "common.h"
#include <iostream>
#include <time.h>

using namespace std;

bool seeded = false;
string getRandomEmail()
{
	if(!seeded)
	{
		srand(time(NULL));
		seeded = true;
	}
	return string("linuxemail") + to_string((int)time(NULL)) + "_" + to_string(rand()) +string("@gmail.com");
}

ManagementClient* CreateManagementClient()
{
	ManagementClient*  managementClient = new ManagementClient(APIURL);

	UserManagementClient* userManagementClient = managementClient->User();

	string email = getRandomEmail();

	RegisterInfo reg;
	reg.Name = "Linux test user";
	reg.Email = email;
	reg.Password = "P@ssw0rd";
	int regRetcode = userManagementClient->Register(reg);

	if(regRetcode!= 0)
		return NULL;

	return managementClient;
}

PlatformTestInput CreatePlatformTestInput()
{
	ManagementClient*  managementClient = CreateManagementClient();
	CompanyManagementClient* companyClient = managementClient->Company();
	ServiceManagementClient* serviceClient = managementClient->Service();
	NetworkManagementClient* networkClient = managementClient->Network();
	DeviceManagementClient* deviceClient = managementClient->Device();
	Company company;
	company.Name = "test company";
	string companyId = companyClient->Create(company);
	Service service;
	service.CompanyId = companyId;
	service.Name = "test service";
	string serviceId = serviceClient->Create(service);
	Network network;
	network.CompanyId = companyId;
	network.ServiceId = serviceId;
	network.Name = "test network";
	string networkId = networkClient->Create(network);
	Device device;
	device.CompanyId = companyId;
	device.ServiceId = serviceId;
	device.NetworkId = networkId;
	device.Name = "test device";
	string deviceId = deviceClient->Create(device);

	Device getDevice = deviceClient->Get(deviceId);

	vector<TelemetryDataSinkParameters> telemetryDataSinkParameters;
	TelemetryDataSinkParameters tdsp;
	tdsp.SinkName = SINKDATA;
	telemetryDataSinkParameters.push_back(tdsp);

	companyClient->UpdateIncomingTelemetryDataSinks(companyId, telemetryDataSinkParameters);

	PlatformTestInput platformTestInput;
	platformTestInput.Dev = getDevice;

	delete managementClient;

	return platformTestInput;
}

ReportingTestInput CreateReportingTestInput()
{
	ManagementClient*  managementClient = CreateManagementClient();
	CompanyManagementClient* companyClient = managementClient->Company();
	ServiceManagementClient* serviceClient = managementClient->Service();
	NetworkManagementClient* networkClient = managementClient->Network();
	DeviceManagementClient* deviceClient = managementClient->Device();
	Company company;
	company.Name = "test company";
	string companyId = companyClient->Create(company);
	Service service;
	service.CompanyId = companyId;
	service.Name = "test service";
	string serviceId = serviceClient->Create(service);
	Network network;
	network.CompanyId = companyId;
	network.ServiceId = serviceId;
	network.Name = "test network";
	string networkId = networkClient->Create(network);
	Device device1;
	device1.CompanyId = companyId;
	device1.ServiceId = serviceId;
	device1.NetworkId = networkId;
	device1.Name = "test device1";
	string device1Id = deviceClient->Create(device1);
	Device device2;
	device2.CompanyId = companyId;
	device2.ServiceId = serviceId;
	device2.NetworkId = networkId;
	device2.Name = "test device2";
	string device2Id = deviceClient->Create(device2);

	Network getNetwork = networkClient->Get(networkId);
	Device getDevice1 = deviceClient->Get(device1Id);
	Device getDevice2 = deviceClient->Get(device2Id);

	vector<TelemetryDataSinkParameters> telemetryDataSinkParameters;
	TelemetryDataSinkParameters tdsp1;
	tdsp1.SinkName = SINKDATA;
	telemetryDataSinkParameters.push_back(tdsp1);
	TelemetryDataSinkParameters tdsp2;
	tdsp2.SinkName = SINKTIMESERIES;
	telemetryDataSinkParameters.push_back(tdsp2);

	companyClient->UpdateIncomingTelemetryDataSinks(companyId, telemetryDataSinkParameters);

	ReportingTestInput reportingTestInput;
	reportingTestInput.Net = getNetwork;
	reportingTestInput.Dev1 = getDevice1;
	reportingTestInput.Dev2 = getDevice2;

	delete managementClient;

	return reportingTestInput;
}

