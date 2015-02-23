#include "gtest/gtest.h"
#include "ManagementClient.h"
#include "common.h"

TEST(DeviceTest, createGetUpdateGetDeleteGet)
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
	string id = deviceClient->Create(device);

	ASSERT_EQ(32, id.length());

	Device getDevice = deviceClient->Get(id);
	
	ASSERT_EQ(id, getDevice.Id);
	ASSERT_EQ(networkId, getDevice.NetworkId);
	ASSERT_EQ(serviceId, getDevice.ServiceId);
	ASSERT_EQ(companyId, getDevice.CompanyId);
	ASSERT_EQ(32, getDevice.DeviceKey.length());
	ASSERT_EQ("test device", getDevice.Name);

	getDevice.Name+= "mod";

	int result = deviceClient->Update(getDevice);
	ASSERT_EQ(0, result);

	Device modDevice = deviceClient->Get(id);
	
	ASSERT_EQ(id, modDevice.Id);
	ASSERT_EQ(networkId, modDevice.NetworkId);
	ASSERT_EQ(serviceId, modDevice.ServiceId);
	ASSERT_EQ(companyId, modDevice.CompanyId);
	ASSERT_EQ(getDevice.DeviceKey, modDevice.DeviceKey);
	ASSERT_EQ("test devicemod", modDevice.Name);

	result = deviceClient->Delete(id);
	ASSERT_EQ(0, result);

	Device delDevice = deviceClient->Get(id);
	ASSERT_TRUE(delDevice.Id.empty());
}

TEST(DeviceTest, createTwoListUnder)
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
	string id1 = deviceClient->Create(device1);

	Device device2;
	device2.CompanyId = companyId;
	device2.ServiceId = serviceId;
	device2.NetworkId = networkId;
	device2.Name = "test device2";
	string id2 = deviceClient->Create(device2);

	vector<Small> devices = networkClient->ListDevices(networkId);

	ASSERT_EQ(2, devices.size());
	ASSERT_EQ(id1, devices[0].Id);
	ASSERT_EQ("test device1", devices[0].Name);
	ASSERT_EQ(id2, devices[1].Id);
	ASSERT_EQ("test device2", devices[1].Name);
}

