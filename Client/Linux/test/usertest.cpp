#include "gtest/gtest.h"
#include "ManagementClient.h"
#include "common.h"

TEST(UserTest, registration)
{
	ManagementClient*  managementClient = new ManagementClient(APIURL);

	UserManagementClient* userManagementClient = managementClient->User();

	string email = getRandomEmail();
	RegisterInfo reg;
	reg.Name = "Linux test user";
	reg.Email = email;
	reg.Password = "P@ssw0rd";
	int regRetcode = userManagementClient->Register(reg);
	bool isLoggedIn = userManagementClient->IsLoggedIn();
	
	ASSERT_EQ(0, regRetcode);
	ASSERT_TRUE(isLoggedIn);
}

TEST(UserTest, getMe)
{
	ManagementClient*  managementClient = new ManagementClient(APIURL);

	UserManagementClient* userManagementClient = managementClient->User();

	string email = getRandomEmail();

	RegisterInfo reg;
	reg.Name = "Linux test user";
	reg.Email = email;
	reg.Password = "P@ssw0rd";
	userManagementClient->Register(reg);
	
	User user = userManagementClient->Get();

	ASSERT_EQ(32, user.Id.length());
	ASSERT_EQ(reg.Name, user.Name);
	ASSERT_EQ(reg.Email, user.Email);
}

TEST(UserTest, getMeNotLoggedIn)
{
	ManagementClient*  managementClient = new ManagementClient(APIURL);

	UserManagementClient* userManagementClient = managementClient->User();

	User user = userManagementClient->Get();
	ASSERT_TRUE(user.Id.empty());
}

TEST(UserTest, loginLogoff)
{
	ManagementClient*  managementClient = new ManagementClient(APIURL);

	UserManagementClient* userManagementClient = managementClient->User();

	string email = getRandomEmail();

	RegisterInfo reg;
	reg.Name = "Linux test user";
	reg.Email = email;
	reg.Password = "P@ssw0rd";
	userManagementClient->Register(reg);

	string regUserId = userManagementClient->Get().Id;

	userManagementClient->Logoff();
	
	LoginInfo login;
	login.Email = email;
	login.Password = reg.Password;

	userManagementClient->Login(login);
	bool isLoggedIn = userManagementClient->IsLoggedIn();
	string loginUserId = userManagementClient->Get().Id;
	
	ASSERT_TRUE(isLoggedIn);
	ASSERT_EQ(regUserId, loginUserId);
}

TEST(UserTest, findUser)
{
	ManagementClient*  managementClient = new ManagementClient(APIURL);

	UserManagementClient* userManagementClient = managementClient->User();

	string email = "stg+" + getRandomEmail();

	RegisterInfo reg;
	reg.Name = "Linux test user";
	reg.Email = email;
	reg.Password = "P@ssw0rd";
	userManagementClient->Register(reg);
	
	User user = userManagementClient->FindUser(email);

	ASSERT_EQ(32, user.Id.length());
	ASSERT_EQ(reg.Name, user.Name);
	ASSERT_EQ(reg.Email, user.Email);
}
