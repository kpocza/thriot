#include "gtest/gtest.h"
#include "ManagementClient.h"
#include "common.h"
#include <algorithm>

TEST(CompanyTest, createGetUpdateGetDeleteGet)
{
	ManagementClient*  managementClient = CreateManagementClient();

	CompanyManagementClient* companyClient = managementClient->Company();

	Company company;
	company.Name = "test company";
	string id = companyClient->Create(company);

	ASSERT_EQ(32, id.length());

	Company getCompany = companyClient->Get(id);
	
	ASSERT_EQ(id, getCompany.Id);
	ASSERT_EQ("test company", getCompany.Name);

	getCompany.Name+= "mod";

	int result = companyClient->Update(getCompany);
	ASSERT_EQ(0, result);

	Company modCompany = companyClient->Get(id);
	
	ASSERT_EQ(id, modCompany.Id);
	ASSERT_EQ("test companymod", modCompany.Name);

	result = companyClient->Delete(id);
	ASSERT_EQ(0, result);

	Company delCompany = companyClient->Get(id);
	ASSERT_TRUE(delCompany.Id.empty());
}

TEST(CompanyTest, createTwoList)
{
	ManagementClient*  managementClient = CreateManagementClient();

	CompanyManagementClient* companyClient = managementClient->Company();

	Company company1;
	company1.Name = "test company1";
	string id1 = companyClient->Create(company1);

	Company company2;
	company2.Name = "test company2";
	string id2 = companyClient->Create(company2);

	vector<Small> companies = companyClient->List();
	sort(companies.begin(), companies.end(), [] (const Small &c1, const Small &c2) {return c1.Name < c2.Name;});

	ASSERT_EQ(2, companies.size());
	ASSERT_EQ(id1, companies[0].Id);
	ASSERT_EQ("test company1", companies[0].Name);
	ASSERT_EQ(id2, companies[1].Id);
	ASSERT_EQ("test company2", companies[1].Name);
}

TEST(CompanyTest, addUserListUsers)
{
	ManagementClient*  managementClient = new ManagementClient(APIURL);

	UserManagementClient* userManagementClient = managementClient->User();

	string email1 = getRandomEmail();
	string email2 = getRandomEmail();

	RegisterInfo reg1;
	reg1.Name = "Linux test user";
	reg1.Email = email1;
	reg1.Password = "P@ssw0rd";
	userManagementClient->Register(reg1);
	RegisterInfo reg2;
	reg2.Name = "Linux test user";
	reg2.Email = email2;
	reg2.Password = "P@ssw0rd";
	userManagementClient->Register(reg2);
	userManagementClient->Logoff();
	
	LoginInfo login;
	login.Email = email1;
	login.Password = reg1.Password;
	userManagementClient->Login(login);

	CompanyManagementClient* companyClient = managementClient->Company();

	Company company;
	company.Name = "test company1";
	string companyId = companyClient->Create(company);

	User user2 = userManagementClient->FindUser(email2);

	companyClient->AddUser(companyId, user2.Id);

	vector<SmallUser> users = companyClient->ListUsers(companyId);
	ASSERT_EQ(2, users.size());

	userManagementClient->Logoff();
	
	login.Email = email2;
	login.Password = reg1.Password;
	userManagementClient->Login(login);

	vector<Small> companies = companyClient->List();
	ASSERT_EQ(1, companies.size());
}

