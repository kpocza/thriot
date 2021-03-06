#include "PlatformClient.h"

namespace Thriot { 
class WebSocketConnection;

namespace Platform {
class PersistentConnectionInternalClient
{
	private:
		string _url;
		WebSocketConnection *_webSocketConnection;
		bool _isLoggedIn;
		bool _isSubscribed;
		SubscriptionType _subscriptionType;
		OnMessageReceived _onMessageReceived;
		string _lastLoginResponse;
		string _lastSubscribeResponse;
		string _lastUnsubscribeResponse;
		string _lastTelemetryDataResponse;
		string _lastSendtoResponse;

	public:
		PersistentConnectionInternalClient(const string& url, WebSocketConnection *webSocketConnection);

		PlatformOperationResult Login(const string& deviceId, const string& apiKey);

		PlatformOperationResult Subscribe(const SubscriptionType subscriptionType, OnMessageReceived onMessageReceived);
		PlatformOperationResult Unsubscribe();

		PlatformOperationResult Close();

		PlatformOperationResult RecordTelemetryData(const string& payload);
		PlatformOperationResult SendMessageTo(const string& deviceId, const string& payload);

		PlatformOperationResult Heartbeat();

		void Spin();
	private:

		void HandlePushedMessage(const string& message);
		void ProcessMessage(const string& message);
		void CommitIfNeeded(); 

		static bool OnMessagePayloadReceived(const void *object, const string& message);
};
}}
