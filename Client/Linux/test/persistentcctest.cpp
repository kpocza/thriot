#include "gtest/gtest.h"
#include "ManagementClient.h"
#include "PlatformClient.h"
#include "common.h"

using namespace Thriot::Platform;

TEST(PersistentConnectionTest, recordTelemetryData)
{
	PlatformTestInput platformTestInput = CreatePlatformTestInput();
	PersistentConnectionClient *persistentConnectionClient = new PersistentConnectionClient();
	PlatformOperationResult por = persistentConnectionClient->Login(WSURL, platformTestInput.Dev.Id, platformTestInput.Dev.DeviceKey);

	ASSERT_EQ(Ok, por);

	int retCode = persistentConnectionClient->RecordTelemetryData("{\"Temperature\": 24, \"Humidity\": 50, \"Source\": \"Linux\"}");

	ASSERT_EQ(0, retCode);

	persistentConnectionClient->Close();
}

TEST(PersistentConnectionTest, loginInvalid)
{
	PlatformTestInput platformTestInput = CreatePlatformTestInput();
	PersistentConnectionClient *persistentConnectionClient = new PersistentConnectionClient();
	PlatformOperationResult por = persistentConnectionClient->Login(WSURL, platformTestInput.Dev.Id, "asdfasdf");

	ASSERT_EQ(LoginInvalid, por);

	persistentConnectionClient->Close();
}

TEST(PersistentConnectionTest, sendto)
{
	PlatformTestInput platformTestInput = CreatePlatformTestInput();
	PersistentConnectionClient *persistentConnectionClient = new PersistentConnectionClient();
	PlatformOperationResult por = persistentConnectionClient->Login(WSURL, platformTestInput.Dev.Id, platformTestInput.Dev.DeviceKey);

	ASSERT_EQ(Ok, por);

	int retCode = persistentConnectionClient->SendMessageTo(platformTestInput.Dev.Id, 
				"{\"Temperature\": 24, \"Humidity\": 50, \"Source\": \"Linux\"}");

	ASSERT_EQ(0, retCode);
	persistentConnectionClient->Close();
}

PushedMessage receivedMessage;

void messageReceived(const PushedMessage& pushedMessage)
{
	receivedMessage = pushedMessage;
}

TEST(PersistentConnectionTest, subscribeUnsubscribe)
{
	PlatformTestInput platformTestInput = CreatePlatformTestInput();
	PersistentConnectionClient *persistentConnectionClient = new PersistentConnectionClient();
	PlatformOperationResult por = persistentConnectionClient->Login(WSURL, platformTestInput.Dev.Id, platformTestInput.Dev.DeviceKey);
	ASSERT_EQ(Ok, por);

	por = persistentConnectionClient->Subscribe(ReceiveAndForget, messageReceived);
	ASSERT_EQ(Ok, por);

	por = persistentConnectionClient->Unsubscribe();
	ASSERT_EQ(Ok, por);
	persistentConnectionClient->Close();
}

TEST(PersistentConnectionTest, sendPlusReceiveAndForget)
{
	PlatformTestInput platformTestInput = CreatePlatformTestInput();
	PersistentConnectionClient *persistentConnectionClient = new PersistentConnectionClient();
	PlatformOperationResult por = persistentConnectionClient->Login(WSURL, platformTestInput.Dev.Id, platformTestInput.Dev.DeviceKey);
	ASSERT_EQ(Ok, por);

	por = persistentConnectionClient->Subscribe(ReceiveAndForget, messageReceived);
	ASSERT_EQ(Ok, por);

	string msg1 = "{\"Temperature\": 24, \"Humidity\": 50, \"Source\": \"Linux1\"}";
	int retCode = persistentConnectionClient->SendMessageTo(platformTestInput.Dev.Id, msg1);
	ASSERT_EQ(0, retCode);

	string msg2 = "{\"Temperature\": 24, \"Humidity\": 50, \"Source\": \"Linux2\"}";
	retCode = persistentConnectionClient->SendMessageTo(platformTestInput.Dev.Id, msg2);
	ASSERT_EQ(0, retCode);

	while(receivedMessage.Payload.empty())
	{
		persistentConnectionClient->Spin();
		usleep(100);
	}
	ASSERT_EQ(msg1, receivedMessage.Payload);
	ASSERT_EQ(0, receivedMessage.MessageId);
	ASSERT_TRUE(receivedMessage.Timestamp > 1000000);
	receivedMessage.Payload.clear();

	while(receivedMessage.Payload.empty())
	{
		persistentConnectionClient->Spin();
		usleep(100);
	}
	ASSERT_EQ(msg2, receivedMessage.Payload);
	ASSERT_EQ(1, receivedMessage.MessageId);
	ASSERT_TRUE(receivedMessage.Timestamp > 1000000);
	receivedMessage.Payload.clear();

	por = persistentConnectionClient->Unsubscribe();
	ASSERT_EQ(Ok, por);
	persistentConnectionClient->Close();
}

TEST(PersistentConnectionTest, sendPlusPeekAndCommit)
{
	PlatformTestInput platformTestInput = CreatePlatformTestInput();
	PersistentConnectionClient *persistentConnectionClient = new PersistentConnectionClient();
	PlatformOperationResult por = persistentConnectionClient->Login(WSURL, platformTestInput.Dev.Id, platformTestInput.Dev.DeviceKey);
	ASSERT_EQ(Ok, por);

	por = persistentConnectionClient->Subscribe(PeekAndCommit, messageReceived);
	ASSERT_EQ(Ok, por);

	string msg1 = "{\"Temperature\": 24, \"Humidity\": 50, \"Source\": \"Linux1\"}";
	int retCode = persistentConnectionClient->SendMessageTo(platformTestInput.Dev.Id, msg1);
	ASSERT_EQ(0, retCode);

	string msg2 = "{\"Temperature\": 24, \"Humidity\": 50, \"Source\": \"Linux2\"}";
	retCode = persistentConnectionClient->SendMessageTo(platformTestInput.Dev.Id, msg2);
	ASSERT_EQ(0, retCode);

	while(receivedMessage.Payload.empty())
	{
		persistentConnectionClient->Spin();
		usleep(100);
	}
	ASSERT_EQ(msg1, receivedMessage.Payload);
	ASSERT_EQ(0, receivedMessage.MessageId);
	ASSERT_TRUE(receivedMessage.Timestamp > 1000000);
	receivedMessage.Payload.clear();

	while(receivedMessage.Payload.empty())
	{
		persistentConnectionClient->Spin();
		usleep(100);
	}
	ASSERT_EQ(msg2, receivedMessage.Payload);
	ASSERT_EQ(1, receivedMessage.MessageId);
	ASSERT_TRUE(receivedMessage.Timestamp > 1000000);
	receivedMessage.Payload.clear();

	por = persistentConnectionClient->Unsubscribe();
	ASSERT_EQ(Ok, por);
	persistentConnectionClient->Close();
}

/*TEST(PersistentConnectionTest, spin)
{
	PlatformTestInput platformTestInput = CreatePlatformTestInput();
	PersistentConnectionClient *persistentConnectionClient = new PersistentConnectionClient();
	PlatformOperationResult por = persistentConnectionClient->Login(WSURL, platformTestInput.Dev.Id, platformTestInput.Dev.DeviceKey);

	ASSERT_EQ(Ok, por);

	while(true)
	{
		persistentConnectionClient->Spin();
		sleep(1);
	}
}*/

