#include "gtest/gtest.h"
#include "ManagementClient.h"
#include "PlatformClient.h"
#include "common.h"

using namespace Thriot::Platform;

TEST(OccasionallyConnectionTest, recordTelemetryData)
{
	PlatformTestInput platformTestInput = CreatePlatformTestInput();

	OccasionallyConnectionClient *occasionallyConnectionClient = new OccasionallyConnectionClient(PAPIURL, 
		platformTestInput.Dev.Id, platformTestInput.Dev.DeviceKey);

	int retCode = occasionallyConnectionClient->RecordTelemetryData("{\"Temperature\": 24, \"Humidity\": 50, \"Source\": \"Linux\"}");

	ASSERT_EQ(0, retCode);
}

TEST(OccasionallyConnectionTest, sendMessageToAndReceiveAndForget)
{
	PlatformTestInput platformTestInput = CreatePlatformTestInput();
	string& deviceId = platformTestInput.Dev.Id;

	OccasionallyConnectionClient *occasionallyConnectionClient = new OccasionallyConnectionClient(PAPIURL, 
		deviceId, platformTestInput.Dev.DeviceKey);

	int retCode = occasionallyConnectionClient->SendMessageTo(deviceId, "string that contains only characters below 0x80");
	ASSERT_EQ(0, retCode);

	PushedMessage pushedMessage = occasionallyConnectionClient->ReceiveAndForgetMessage();

	ASSERT_TRUE(pushedMessage.MessageId >= 0);
	ASSERT_EQ("string that contains only characters below 0x80", pushedMessage.Payload);
	ASSERT_TRUE(pushedMessage.Timestamp >= 1420848888L);
	ASSERT_EQ(deviceId, pushedMessage.SenderDeviceId);
}

TEST(OccasionallyConnectionTest, receiveAndForgetWoMessage)
{
	PlatformTestInput platformTestInput = CreatePlatformTestInput();
	string& deviceId = platformTestInput.Dev.Id;

	OccasionallyConnectionClient *occasionallyConnectionClient = new OccasionallyConnectionClient(PAPIURL, 
		deviceId, platformTestInput.Dev.DeviceKey);

	PushedMessage pushedMessage = occasionallyConnectionClient->ReceiveAndForgetMessage();

	ASSERT_EQ(NOMESSAGERECEIVED, pushedMessage.MessageId);
}

TEST(OccasionallyConnectionTest, sendMessageToAndPeekCommit)
{
	PlatformTestInput platformTestInput = CreatePlatformTestInput();
	string& deviceId = platformTestInput.Dev.Id;

	OccasionallyConnectionClient *occasionallyConnectionClient = new OccasionallyConnectionClient(PAPIURL, 
		deviceId, platformTestInput.Dev.DeviceKey);

	int retCode = occasionallyConnectionClient->SendMessageTo(deviceId, "msg1");
	ASSERT_EQ(0, retCode);
	retCode = occasionallyConnectionClient->SendMessageTo(deviceId, "msg2");
	ASSERT_EQ(0, retCode);

	PushedMessage pushedMessage1 = occasionallyConnectionClient->PeekMessage();

	ASSERT_TRUE(pushedMessage1.MessageId >= 0);
	ASSERT_EQ("msg1", pushedMessage1.Payload);
	ASSERT_TRUE(pushedMessage1.Timestamp >= 1420848888L);
	ASSERT_EQ(deviceId, pushedMessage1.SenderDeviceId);

	PushedMessage pushedMessage2 = occasionallyConnectionClient->PeekMessage();

	ASSERT_EQ(pushedMessage1.MessageId, pushedMessage2.MessageId);
	ASSERT_EQ("msg1", pushedMessage2.Payload);
	ASSERT_TRUE(pushedMessage2.Timestamp >= 1420848888L);
	ASSERT_EQ(deviceId, pushedMessage2.SenderDeviceId);

	retCode = occasionallyConnectionClient->CommitMessage();
	ASSERT_EQ(0, retCode);

	PushedMessage pushedMessage3 = occasionallyConnectionClient->PeekMessage();
	ASSERT_EQ("msg2", pushedMessage3.Payload);
	ASSERT_EQ(deviceId, pushedMessage3.SenderDeviceId);
	
	retCode = occasionallyConnectionClient->CommitMessage();
	ASSERT_EQ(0, retCode);
}

TEST(OccasionallyConnectionTest, peekWoMessage)
{
	PlatformTestInput platformTestInput = CreatePlatformTestInput();
	string& deviceId = platformTestInput.Dev.Id;

	OccasionallyConnectionClient *occasionallyConnectionClient = new OccasionallyConnectionClient(PAPIURL, 
		deviceId, platformTestInput.Dev.DeviceKey);

	PushedMessage pushedMessage = occasionallyConnectionClient->PeekMessage();

	ASSERT_EQ(NOMESSAGERECEIVED, pushedMessage.MessageId);
}

TEST(OccasionallyConnectionTest, commitNothing)
{
	PlatformTestInput platformTestInput = CreatePlatformTestInput();
	string& deviceId = platformTestInput.Dev.Id;

	OccasionallyConnectionClient *occasionallyConnectionClient = new OccasionallyConnectionClient(PAPIURL, 
		deviceId, platformTestInput.Dev.DeviceKey);

	int retCode = occasionallyConnectionClient->CommitMessage();
	ASSERT_EQ(0, retCode);
}


