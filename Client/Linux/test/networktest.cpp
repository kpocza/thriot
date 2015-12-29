#include "gtest/gtest.h"
#include "ManagementClient.h"
#include "common.h"
#include <algorithm>

TEST(NetworkTest, createGetUpdateGetDeleteGetUnderService)
{
	ManagementClient*  managementClient = CreateManagementClient();
	CompanyManagementClient* companyClient = managementClient->Company();
	ServiceManagementClient* serviceClient = managementClient->Service();
	NetworkManagementClient* networkClient = managementClient->Network();
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
	string id = networkClient->Create(network);

	ASSERT_EQ(32, id.length());

	Network getNetwork = networkClient->Get(id);
	ASSERT_EQ(id, getNetwork.Id);
	ASSERT_EQ(serviceId, getNetwork.ServiceId);
	ASSERT_EQ(companyId, getNetwork.CompanyId);
	ASSERT_TRUE(getNetwork.ParentNetworkId.empty());
	ASSERT_EQ("test network", getNetwork.Name);
	ASSERT_EQ(32, getNetwork.NetworkKey.length());

	getNetwork.Name+= "mod";

	int result = networkClient->Update(getNetwork);
	ASSERT_EQ(0, result);

	Network modNetwork = networkClient->Get(id);
	
	ASSERT_EQ(id, modNetwork.Id);
	ASSERT_EQ(serviceId, modNetwork.ServiceId);
	ASSERT_EQ(companyId, modNetwork.CompanyId);
	ASSERT_TRUE(modNetwork.ParentNetworkId.empty());
	ASSERT_EQ("test networkmod", modNetwork.Name);

	result = networkClient->Delete(id);
	ASSERT_EQ(0, result);

	Network delNetwork = networkClient->Get(id);
	ASSERT_TRUE(delNetwork.Id.empty());
}

TEST(NetworkTest, createTwoListUnderService)
{
	ManagementClient*  managementClient = CreateManagementClient();
	CompanyManagementClient* companyClient = managementClient->Company();
	ServiceManagementClient* serviceClient = managementClient->Service();
	NetworkManagementClient* networkClient = managementClient->Network();
	Company company;
	company.Name = "test company";
	string companyId = companyClient->Create(company);
	Service service;
	service.CompanyId = companyId;
	service.Name = "test service";
	string serviceId = serviceClient->Create(service);

	Network network1;
	network1.CompanyId = companyId;
	network1.ServiceId = serviceId;
	network1.Name = "test network1";
	string id1 = networkClient->Create(network1);

	Network network2;
	network2.CompanyId = companyId;
	network2.ServiceId = serviceId;
	network2.Name = "test network2";
	string id2 = networkClient->Create(network2);

	vector<Small> networks = serviceClient->ListNetworks(serviceId);
	sort(networks.begin(), networks.end(), [] (const Small &n1, const Small &n2) {return n1.Name < n2.Name;});

	ASSERT_EQ(2, networks.size());
	ASSERT_EQ(id1, networks[0].Id);
	ASSERT_EQ("test network1", networks[0].Name);
	ASSERT_EQ(id2, networks[1].Id);
	ASSERT_EQ("test network2", networks[1].Name);
}

TEST(NetworkTest, createGetUpdateGetDeleteGetUnderNetwork)
{
	ManagementClient*  managementClient = CreateManagementClient();
	CompanyManagementClient* companyClient = managementClient->Company();
	ServiceManagementClient* serviceClient = managementClient->Service();
	NetworkManagementClient* networkClient = managementClient->Network();
	Company company;
	company.Name = "test company";
	string companyId = companyClient->Create(company);
	Service service;
	service.CompanyId = companyId;
	service.Name = "test service";
	string serviceId = serviceClient->Create(service);
	Network parentNetwork;
	parentNetwork.CompanyId = companyId;
	parentNetwork.ServiceId = serviceId;
	parentNetwork.Name = "test network";
	string parentNetworkId = networkClient->Create(parentNetwork);

	Network network;
	network.CompanyId = companyId;
	network.ServiceId = serviceId;
	network.ParentNetworkId = parentNetworkId;
	network.Name = "test network";
	string id = networkClient->Create(network);

	ASSERT_EQ(32, id.length());

	Network getNetwork = networkClient->Get(id);
	
	ASSERT_EQ(id, getNetwork.Id);
	ASSERT_EQ(parentNetworkId, getNetwork.ParentNetworkId);
	ASSERT_EQ(serviceId, getNetwork.ServiceId);
	ASSERT_EQ(companyId, getNetwork.CompanyId);
	ASSERT_EQ("test network", getNetwork.Name);

	getNetwork.Name+= "mod";

	int result = networkClient->Update(getNetwork);
	ASSERT_EQ(0, result);

	Network modNetwork = networkClient->Get(id);
	
	ASSERT_EQ(id, modNetwork.Id);
	ASSERT_EQ(parentNetworkId, modNetwork.ParentNetworkId);
	ASSERT_EQ(serviceId, modNetwork.ServiceId);
	ASSERT_EQ(companyId, modNetwork.CompanyId);
	ASSERT_EQ("test networkmod", modNetwork.Name);

	result = networkClient->Delete(id);
	ASSERT_EQ(0, result);

	Network delNetwork = networkClient->Get(id);
	ASSERT_TRUE(delNetwork.Id.empty());
}

TEST(NetworkTest, createTwoListUnderNetwork)
{
	ManagementClient*  managementClient = CreateManagementClient();
	CompanyManagementClient* companyClient = managementClient->Company();
	ServiceManagementClient* serviceClient = managementClient->Service();
	NetworkManagementClient* networkClient = managementClient->Network();
	Company company;
	company.Name = "test company";
	string companyId = companyClient->Create(company);
	Service service;
	service.CompanyId = companyId;
	service.Name = "test service";
	string serviceId = serviceClient->Create(service);
	Network parentNetwork;
	parentNetwork.CompanyId = companyId;
	parentNetwork.ServiceId = serviceId;
	parentNetwork.Name = "test network";
	string parentNetworkId = networkClient->Create(parentNetwork);

	Network network1;
	network1.CompanyId = companyId;
	network1.ServiceId = serviceId;
	network1.ParentNetworkId = parentNetworkId;
	network1.Name = "test network1";
	string id1 = networkClient->Create(network1);

	Network network2;
	network2.CompanyId = companyId;
	network2.ServiceId = serviceId;
	network2.ParentNetworkId = parentNetworkId;
	network2.Name = "test network2";
	string id2 = networkClient->Create(network2);

	vector<Small> networks = networkClient->ListNetworks(parentNetworkId);
	sort(networks.begin(), networks.end(), [] (const Small &n1, const Small &n2) {return n1.Name < n2.Name;});

	ASSERT_EQ(2, networks.size());
	ASSERT_EQ(id1, networks[0].Id);
	ASSERT_EQ("test network1", networks[0].Name);
	ASSERT_EQ(id2, networks[1].Id);
	ASSERT_EQ("test network2", networks[1].Name);
}

