#include "gtest/gtest.h"
#include "ManagementClient.h"
#include "common.h"
#include <algorithm>

TEST(ServiceTest, createGetUpdateGetDeleteGet)
{
	ManagementClient*  managementClient = CreateManagementClient();
	CompanyManagementClient* companyClient = managementClient->Company();
	ServiceManagementClient* serviceClient = managementClient->Service();
	Company company;
	company.Name = "test company";
	string companyId = companyClient->Create(company);

	Service service;
	service.CompanyId = companyId;
	service.Name = "test service";
	string id = serviceClient->Create(service);

	ASSERT_EQ(32, id.length());

	Service getService = serviceClient->Get(id);
	
	ASSERT_EQ(id, getService.Id);
	ASSERT_EQ(companyId, getService.CompanyId);
	ASSERT_EQ(32, getService.ApiKey.length());
	ASSERT_EQ("test service", getService.Name);

	getService.Name+= "mod";

	int result = serviceClient->Update(getService);
	ASSERT_EQ(0, result);

	Service modService = serviceClient->Get(id);
	
	ASSERT_EQ(id, modService.Id);
	ASSERT_EQ(companyId, modService.CompanyId);
	ASSERT_EQ(getService.ApiKey, modService.ApiKey);
	ASSERT_EQ("test servicemod", modService.Name);

	result = serviceClient->Delete(id);
	ASSERT_EQ(0, result);

	Service delService = serviceClient->Get(id);
	ASSERT_TRUE(delService.Id.empty());
}

TEST(ServiceTest, createTwoList)
{
	ManagementClient*  managementClient = CreateManagementClient();
	CompanyManagementClient* companyClient = managementClient->Company();
	ServiceManagementClient* serviceClient = managementClient->Service();
	Company company;
	company.Name = "test company";
	string companyId = companyClient->Create(company);

	Service service1;
	service1.CompanyId = companyId;
	service1.Name = "test service1";
	string id1 = serviceClient->Create(service1);

	Service service2;
	service2.CompanyId = companyId;
	service2.Name = "test service2";
	string id2 = serviceClient->Create(service2);

	vector<Small> services = companyClient->ListServices(companyId);
	sort(services.begin(), services.end(), [] (const Small &s1, const Small &s2) {return s1.Name < s2.Name;});

	ASSERT_EQ(2, services.size());
	ASSERT_EQ(id1, services[0].Id);
	ASSERT_EQ("test service1", services[0].Name);
	ASSERT_EQ(id2, services[1].Id);
	ASSERT_EQ("test service2", services[1].Name);
}

