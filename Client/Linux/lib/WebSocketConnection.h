#include <string>
#include "libwebsockets.h"

using namespace std;

namespace Thriot
{
typedef bool (*MessagePayloadReceivedFunc)(const void *object, const string& message);

class WebSocketConnection
{
	private:
		MessagePayloadReceivedFunc _onMessage;
		void *_object;
		static lws_context *_context;
		struct lws *_wsi;
		string _partialReceived;
		string _messageToSend;
		string _messageReceived;
		bool _isConnected;
		static int lws_callback(struct lws *wsi, enum lws_callback_reasons reason, 
			void *userdata, void *in, size_t len);
	public:
		bool Connect(const string& url);
		void OnMessage(void *object, MessagePayloadReceivedFunc onMessage);
		bool IsDisconnected();
		void Send(const string& message);
		string WaitResponse(const int timeout);
		void Spin();
		void Close();
};
}

