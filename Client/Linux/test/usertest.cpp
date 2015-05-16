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

TEST(UserTest, tryActivate)
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
	
	ActivateInfo activate;
	activate.UserId = regUserId;
	activate.ActivationCode = "12345678901234567890123456789012";

	int returnCode = userManagementClient->Activate(activate);
	
	ASSERT_EQ(403, returnCode);
}

TEST(UserTest, tryResendActivationEmail)
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
	
	int returnCode = userManagementClient->ResendActivationEmail(email);
	
	ASSERT_EQ(403, returnCode);
}

//TEST(UserTest, SendForgotPasswordEmail)
//{
//	ManagementClient*  managementClient = new ManagementClient(APIURL);
//
//	UserManagementClient* userManagementClient = managementClient->User();
//
//	string email = getRandomEmail();
//
//	RegisterInfo reg;
//	reg.Name = "Linux test user";
//	reg.Email = email;
//	reg.Password = "P@ssw0rd";
//	userManagementClient->Register(reg);
//
//	string regUserId = userManagementClient->Get().Id;
//
//	userManagementClient->Logoff();
//	
//	int returnCode = userManagementClient->SendForgotPasswordEmail(email);
//	
//	ASSERT_EQ(0, returnCode);
//}

TEST(UserTest, tryResetPassword)
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
	
	ResetPasswordInfo resetPassword;
	resetPassword.UserId = regUserId;
	resetPassword.ConfirmationCode = "12345678901234567890123456789012";
	resetPassword.Password = "p@asswd2";

	int returnCode = userManagementClient->ResetPassword(resetPassword);
	
	ASSERT_EQ(403, returnCode);
}

TEST(UserTest, tryResetPasswordInvalidUser)
{
	ManagementClient*  managementClient = new ManagementClient(APIURL);

	UserManagementClient* userManagementClient = managementClient->User();
	
	ResetPasswordInfo resetPassword;
	resetPassword.UserId = "12345678901234567890123456789012";
	resetPassword.ConfirmationCode = "12345678901234567890123456789012";
	resetPassword.Password = "p@asswd2";

	int returnCode = userManagementClient->ResetPassword(resetPassword);
	
	ASSERT_EQ(404, returnCode);
}

TEST(UserTest, changePasswordTest)
{
	ManagementClient*  managementClient = new ManagementClient(APIURL);

	UserManagementClient* userManagementClient = managementClient->User();

	string email = getRandomEmail();

	RegisterInfo reg;
	reg.Name = "Linux test user";
	reg.Email = email;
	reg.Password = "P@ssw0rd";
	userManagementClient->Register(reg);

	ChangePasswordInfo changePassword;
	changePassword.CurrentPassword = reg.Password;
	changePassword.NewPassword = "P@ssw0rd2";
	userManagementClient->ChangePassword(changePassword);

	userManagementClient->Logoff();
	
	LoginInfo login;
	login.Email = email;
	login.Password = changePassword.NewPassword;

	userManagementClient->Login(login);
	bool isLoggedIn = userManagementClient->IsLoggedIn();
	
	ASSERT_TRUE(isLoggedIn);
}

TEST(UserTest, tryChangePasswordInvalidCurrentPasswordTest)
{
	ManagementClient*  managementClient = new ManagementClient(APIURL);

	UserManagementClient* userManagementClient = managementClient->User();

	string email = getRandomEmail();

	RegisterInfo reg;
	reg.Name = "Linux test user";
	reg.Email = email;
	reg.Password = "P@ssw0rd";
	userManagementClient->Register(reg);

	ChangePasswordInfo changePassword;
	changePassword.CurrentPassword = "qwioeruasdkjfhaskjfh";
	changePassword.NewPassword = "P@ssw0rd2";
	int retCode = userManagementClient->ChangePassword(changePassword);

	ASSERT_EQ(401, retCode);
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
