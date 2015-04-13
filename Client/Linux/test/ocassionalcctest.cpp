#include "gtest/gtest.h"
#include "ManagementClient.h"
#include "PlatformClient.h"
#include "common.h"

using namespace Thriot::Platform;

TEST(OcassionalConnectionTest, recordTelemetryData)
{
	PlatformTestInput platformTestInput = CreatePlatformTestInput();

	OcassionalConnectionClient *ocassionalConnectionClient = new OcassionalConnectionClient(PAPIURL, 
		platformTestInput.Dev.Id, platformTestInput.Dev.DeviceKey);

	int retCode = ocassionalConnectionClient->RecordTelemetryData("{\"Temperature\": 24, \"Humidity\": 50, \"Source\": \"Linux\"}");

	ASSERT_EQ(0, retCode);
}

TEST(OcassionalConnectionTest, sendMessageToAndReceiveAndForget)
{
	PlatformTestInput platformTestInput = CreatePlatformTestInput();
	string& deviceId = platformTestInput.Dev.Id;

	OcassionalConnectionClient *ocassionalConnectionClient = new OcassionalConnectionClient(PAPIURL, 
		deviceId, platformTestInput.Dev.DeviceKey);

	int retCode = ocassionalConnectionClient->SendMessageTo(deviceId, "string that contains only characters below 0x80");
	ASSERT_EQ(0, retCode);

	PushedMessage pushedMessage = ocassionalConnectionClient->ReceiveAndForgetMessage();

	ASSERT_TRUE(pushedMessage.MessageId >= 0);
	ASSERT_EQ("string that contains only characters below 0x80", pushedMessage.Payload);
	ASSERT_TRUE(pushedMessage.Timestamp >= 1420848888L);
}

TEST(OcassionalConnectionTest, receiveAndForgetWoMessage)
{
	PlatformTestInput platformTestInput = CreatePlatformTestInput();
	string& deviceId = platformTestInput.Dev.Id;

	OcassionalConnectionClient *ocassionalConnectionClient = new OcassionalConnectionClient(PAPIURL, 
		deviceId, platformTestInput.Dev.DeviceKey);

	PushedMessage pushedMessage = ocassionalConnectionClient->ReceiveAndForgetMessage();

	ASSERT_EQ(NOMESSAGERECEIVED, pushedMessage.MessageId);
}

TEST(OcassionalConnectionTest, sendMessageToAndPeekCommit)
{
	PlatformTestInput platformTestInput = CreatePlatformTestInput();
	string& deviceId = platformTestInput.Dev.Id;

	OcassionalConnectionClient *ocassionalConnectionClient = new OcassionalConnectionClient(PAPIURL, 
		deviceId, platformTestInput.Dev.DeviceKey);

	int retCode = ocassionalConnectionClient->SendMessageTo(deviceId, "msg1");
	ASSERT_EQ(0, retCode);
	retCode = ocassionalConnectionClient->SendMessageTo(deviceId, "msg2");
	ASSERT_EQ(0, retCode);

	PushedMessage pushedMessage1 = ocassionalConnectionClient->PeekMessage();

	ASSERT_TRUE(pushedMessage1.MessageId >= 0);
	ASSERT_EQ("msg1", pushedMessage1.Payload);
	ASSERT_TRUE(pushedMessage1.Timestamp >= 1420848888L);

	PushedMessage pushedMessage2 = ocassionalConnectionClient->PeekMessage();

	ASSERT_EQ(pushedMessage1.MessageId, pushedMessage2.MessageId);
	ASSERT_EQ("msg1", pushedMessage2.Payload);
	ASSERT_TRUE(pushedMessage2.Timestamp >= 1420848888L);

	retCode = ocassionalConnectionClient->CommitMessage();
	ASSERT_EQ(0, retCode);

	PushedMessage pushedMessage3 = ocassionalConnectionClient->PeekMessage();
	ASSERT_EQ("msg2", pushedMessage3.Payload);
	
	retCode = ocassionalConnectionClient->CommitMessage();
	ASSERT_EQ(0, retCode);
}

TEST(OcassionalConnectionTest, peekWoMessage)
{
	PlatformTestInput platformTestInput = CreatePlatformTestInput();
	string& deviceId = platformTestInput.Dev.Id;

	OcassionalConnectionClient *ocassionalConnectionClient = new OcassionalConnectionClient(PAPIURL, 
		deviceId, platformTestInput.Dev.DeviceKey);

	PushedMessage pushedMessage = ocassionalConnectionClient->PeekMessage();

	ASSERT_EQ(NOMESSAGERECEIVED, pushedMessage.MessageId);
}

TEST(OcassionalConnectionTest, commitNothing)
{
	PlatformTestInput platformTestInput = CreatePlatformTestInput();
	string& deviceId = platformTestInput.Dev.Id;

	OcassionalConnectionClient *ocassionalConnectionClient = new OcassionalConnectionClient(PAPIURL, 
		deviceId, platformTestInput.Dev.DeviceKey);

	int retCode = ocassionalConnectionClient->CommitMessage();
	ASSERT_EQ(0, retCode);
}


