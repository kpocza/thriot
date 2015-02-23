#include <string>

using namespace std;

typedef bool (*MessagePayloadReceivedFunc)(const void *object, const string& message);
struct libwebsocket_context;
struct libwebsocket;

class WebSocketConnection
{
	private:
		MessagePayloadReceivedFunc _onMessage;
		void *_object;
		libwebsocket_context *_context;
		struct libwebsocket *_wsi;
		string _partialReceived;
		string _messageToSend;
		string _messageReceived;
		bool _isConnected;
	public:
		bool Connect(const string& url);
		void OnMessage(void *object, MessagePayloadReceivedFunc onMessage);
		bool IsDisconnected();
		void Send(const string& message);
		string WaitResponse(const int timeout);
		void Spin();
		void Close();
		static int lws_callback(struct libwebsocket_context *context, struct libwebsocket *wsi,
              int reason, void *userdata, void *in, size_t len);
};
