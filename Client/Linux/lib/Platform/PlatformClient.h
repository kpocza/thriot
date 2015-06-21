#pragma once

#include <string>
#include <inttypes.h>

using namespace std;

namespace Thriot
{
class RestConnection;

namespace Platform
{
class PersistentConnectionInternalClient;

/** A message that was received from other device to the current device */
struct PushedMessage
{
	/** Device-level incremental, unique message identifier */
	int32_t MessageId;

	/** Unix timestamp. Seconds elapsed since 1970.1.1. */
	int64_t Timestamp;

	/** Sender device's id */
	string SenderDeviceId;

	/** Message payload. Maximum of 512 characters */
	string Payload;
};

/** Subscription QoS level for the persistent connected client */
enum SubscriptionType
{
	/** The message is deleted from the service after sending it to the client. QoS 0 from the receiver's perspective. */
	ReceiveAndForget,
	/** A commit command should be issued from the client to commit message download. QoS 1 from the receiver's perspective */
	PeekAndCommit
};

/** Persistent connection client status codes */
enum PlatformOperationResult
{
	/** Everything is fine */
	Ok = 0,
	/** The persistent connection has benn desconnected */
	Disconnected,
	/** Already connected. Not needed to connect again */
	ConnectedAlready,
	/** Unable to connect */
	ConnectionFailed,
	/** Operation time. 10 secs or similar timeouts are set in the system for an operation.*/
	Timeout,
	/** Login required to access the operation */
	LoginRequired,
	/** Invalid login parameters */
	LoginInvalid,
	/** ALrady subscribed */
	SubscribedAlready,
	/** Invalid subscription parameters */
	SubscribeInvalid,
	/** Subscription required to use the operations */
	SubscriptionRequired,
	/** Trying to unsubscribe from a not subscribed channel */
	UnsubscribeNotsubscribed,
	/** Invalid unsubscribe call */
	UnsubscribeInvalid,
	/** Invalid telemetry data to record */
	TelemetryDataInvalid,
	/** Invalid send to operation */
	SendToInvalid,
	/** Invalid operation */
	InvalidOperation,
	/** Program with operation arguments */
	ArgumentOutOfRange,
	/** Invalid heatbeat response */
	HeartbeatInvalid
};

typedef void (*OnMessageReceived)(const PushedMessage& message);

#define NOMESSAGERECEIVED -1

/** 
Connection type that represent an occasionally connection client. 
This client connects only to the service when it needs to do something.
Everytime a REST call is issued with the full authentication flow. 
*/
class OccasionallyConnectionClient
{
	private:
		RestConnection *_restConnection;

	public:
		OccasionallyConnectionClient(const string& baseUrl, const string& deviceId, const string& apiKey);

		int RecordTelemetryData(const string& message);

		int SendMessageTo(const string& deviceId, const string& message);
		PushedMessage ReceiveAndForgetMessage();
		PushedMessage PeekMessage();
		int CommitMessage();

	private:
		PushedMessage Receive(const string& url);
};

/**
Connection type that has a persistently open websocket connection.
The messages are pushed to the device. The channel is authenticated persistently.
*/
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

		const long _heartbeatTimeSpan;
		long long _lastHeartbeatTime;

	public:
		PersistentConnectionClient(const string& url, const int maxRetryCount = 5);

		PlatformOperationResult Login(const string& deviceId, const string& apiKey);

		PlatformOperationResult Subscribe(const SubscriptionType subscriptionType, OnMessageReceived onMessageReceived);
		PlatformOperationResult Unsubscribe();

		void Close();

		PlatformOperationResult RecordTelemetryData(const string& payload);
		PlatformOperationResult SendMessageTo(const string& deviceId, const string& payload);

		PlatformOperationResult Spin();

	private:
		void InitializeClient();
		void Relogin();
		void Wait();
		void RecordHeartbeat();
};
}}
