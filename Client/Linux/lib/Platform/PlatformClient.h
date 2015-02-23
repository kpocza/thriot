#pragma once

#include <string>
#include <inttypes.h>

using namespace std;

struct PushedMessage
{
	int32_t MessageId;

	int64_t Timestamp;

	string Payload;
};

enum SubscriptionType
{
	ReceiveAndForget,
	PeekAndCommit
};

enum PlatformOperationResult
{
	Ok = 0,
	Disconnected,
	ConnectedAlready,
	ConnectionFailed,
	Timeout,
	LoginRequired,
	LoginInvalid,
	SubscribedAlready,
	SubscribeInvalid,
	SubscriptionRequired,
	UnsubscribeNotsubscribed,
	UnsubscribeInvalid,
	MessageInvalid,
	SendToInvalid,
	InvalidOperation,
	ArgumentOutOfRange
};

typedef void (*OnMessageReceived)(const PushedMessage& message);

#define NOMESSAGERECEIVED -1

class RestConnection;
class PersistentConnectionInternalClient;

class OcassionalConnectionClient
{
	private:
		RestConnection *_restConnection;

	public:
		OcassionalConnectionClient(const string& baseUrl, const string& deviceId, const string& apiKey);

		int RecordTelemetryData(const string& message);

		int SendMessageTo(const string& deviceId, const string& message);
		PushedMessage ReceiveAndForgetMessage();
		PushedMessage PeekMessage();
		int CommitMessage();

	private:
		PushedMessage Receive(const string& url);
};

class PersistentConnectionClient
{
	private:
		PersistentConnectionInternalClient* _persistentConnectionInternalClient;
		int _maxRetryCount;
		string _url;
		string _deviceId;
		string _apiKey;
		bool _isLoggedIn;
		bool _isSubscribed;
		SubscriptionType _subscriptionType;
		OnMessageReceived _onMessageReceived;

	public:
		PersistentConnectionClient(const int maxRetryCount = 5);

		PlatformOperationResult Login(const string& url, const string& deviceId, const string& apiKey);

		PlatformOperationResult Subscribe(const SubscriptionType subscriptionType, OnMessageReceived onMessageReceived);
		PlatformOperationResult Unsubscribe();

		void Close();

		PlatformOperationResult RecordTelemetryData(const string& payload);
		PlatformOperationResult SendMessageTo(const string& deviceId, const string& payload);

		void Spin();

	private:
		void InitializeClient();
		void Relogin();
		void Wait();
};

