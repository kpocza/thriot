#include "gtest/gtest.h"
#include "ManagementClient.h"

// dirty hack to expose private fields to unit test
#define private public

#include "PlatformClient.h"
#include "common.h"

using namespace Thriot::Platform;

TEST(PersistentConnectionTest, recordTelemetryData)
{
	PlatformTestInput platformTestInput = CreatePlatformTestInput();
	PersistentConnectionClient *persistentConnectionClient = new PersistentConnectionClient(WSURL);
	PlatformOperationResult por = persistentConnectionClient->Login(platformTestInput.Dev.Id, platformTestInput.Dev.DeviceKey);

	ASSERT_EQ(Ok, por);

	int retCode = persistentConnectionClient->RecordTelemetryData("{\"Temperature\": 24, \"Humidity\": 50, \"Source\": \"Linux\"}");

	ASSERT_EQ(0, retCode);

	persistentConnectionClient->Close();
}

TEST(PersistentConnectionTest, loginInvalid)
{
	PlatformTestInput platformTestInput = CreatePlatformTestInput();
	PersistentConnectionClient *persistentConnectionClient = new PersistentConnectionClient(WSURL);
	PlatformOperationResult por = persistentConnectionClient->Login(platformTestInput.Dev.Id, "asdfasdf");

	ASSERT_EQ(LoginInvalid, por);

	persistentConnectionClient->Close();
}

TEST(PersistentConnectionTest, sendto)
{
	PlatformTestInput platformTestInput = CreatePlatformTestInput();
	PersistentConnectionClient *persistentConnectionClient = new PersistentConnectionClient(WSURL);
	PlatformOperationResult por = persistentConnectionClient->Login(platformTestInput.Dev.Id, platformTestInput.Dev.DeviceKey);

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
	PersistentConnectionClient *persistentConnectionClient = new PersistentConnectionClient(WSURL);
	PlatformOperationResult por = persistentConnectionClient->Login(platformTestInput.Dev.Id, platformTestInput.Dev.DeviceKey);
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
	PersistentConnectionClient *persistentConnectionClient = new PersistentConnectionClient(WSURL);
	PlatformOperationResult por = persistentConnectionClient->Login(platformTestInput.Dev.Id, platformTestInput.Dev.DeviceKey);
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
	ASSERT_EQ(platformTestInput.Dev.Id, receivedMessage.SenderDeviceId);
	receivedMessage.Payload.clear();

	while(receivedMessage.Payload.empty())
	{
		persistentConnectionClient->Spin();
		usleep(100);
	}
	ASSERT_EQ(msg2, receivedMessage.Payload);
	ASSERT_EQ(1, receivedMessage.MessageId);
	ASSERT_TRUE(receivedMessage.Timestamp > 1000000);
	ASSERT_EQ(platformTestInput.Dev.Id, receivedMessage.SenderDeviceId);
	receivedMessage.Payload.clear();

	por = persistentConnectionClient->Unsubscribe();
	ASSERT_EQ(Ok, por);
	persistentConnectionClient->Close();
}

TEST(PersistentConnectionTest, sendPlusPeekAndCommit)
{
	PlatformTestInput platformTestInput = CreatePlatformTestInput();
	PersistentConnectionClient *persistentConnectionClient = new PersistentConnectionClient(WSURL);
	PlatformOperationResult por = persistentConnectionClient->Login(platformTestInput.Dev.Id, platformTestInput.Dev.DeviceKey);
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
	ASSERT_EQ(platformTestInput.Dev.Id, receivedMessage.SenderDeviceId);
	receivedMessage.Payload.clear();

	while(receivedMessage.Payload.empty())
	{
		persistentConnectionClient->Spin();
		usleep(100);
	}
	ASSERT_EQ(msg2, receivedMessage.Payload);
	ASSERT_EQ(1, receivedMessage.MessageId);
	ASSERT_TRUE(receivedMessage.Timestamp > 1000000);
	ASSERT_EQ(platformTestInput.Dev.Id, receivedMessage.SenderDeviceId);
	receivedMessage.Payload.clear();

	por = persistentConnectionClient->Unsubscribe();
	ASSERT_EQ(Ok, por);
	persistentConnectionClient->Close();
}

TEST(PersistentConnectionTest, heartbeatTest)
{
	PlatformTestInput platformTestInput = CreatePlatformTestInput();
	PersistentConnectionClient *persistentConnectionClient = new PersistentConnectionClient(WSURL);

	long long lastHeartbeatBeforeLogin = persistentConnectionClient->_lastHeartbeatTime;

	PlatformOperationResult por = persistentConnectionClient->Login(platformTestInput.Dev.Id, platformTestInput.Dev.DeviceKey);
	ASSERT_EQ(Ok, por);

	long long lastHeartbeatAfterLogin = persistentConnectionClient->_lastHeartbeatTime;
	usleep(1000*1000);

	int retCode = persistentConnectionClient->RecordTelemetryData("{\"Temperature\": 24, \"Humidity\": 50, \"Source\": \"Linux\"}");
	ASSERT_EQ(0, retCode);

	long long lastHeartbeatAfterOperation = persistentConnectionClient->_lastHeartbeatTime;

	ASSERT_TRUE(lastHeartbeatBeforeLogin < lastHeartbeatAfterLogin);
	ASSERT_TRUE(lastHeartbeatAfterLogin < lastHeartbeatAfterOperation);

	persistentConnectionClient->Spin();
	long long afterNearspin = persistentConnectionClient->_lastHeartbeatTime;

	ASSERT_EQ(lastHeartbeatAfterOperation, afterNearspin);

	persistentConnectionClient->_lastHeartbeatTime = time(NULL)-1000;

	usleep(1000*1000);
	persistentConnectionClient->Spin();

	long long afterFarspin = persistentConnectionClient->_lastHeartbeatTime;
	ASSERT_TRUE(afterNearspin < afterFarspin);
}

/*TEST(PersistentConnectionTest, spin)
{
	PlatformTestInput platformTestInput = CreatePlatformTestInput();
	PersistentConnectionClient *persistentConnectionClient = new PersistentConnectionClient(WSURL);
	PlatformOperationResult por = persistentConnectionClient->Login(platformTestInput.Dev.Id, platformTestInput.Dev.DeviceKey);

	ASSERT_EQ(Ok, por);

	while(true)
	{
		persistentConnectionClient->Spin();
		sleep(1);
	}
}*/

