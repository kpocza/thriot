#include "WebSocketConnection.h"
#include "libwebsockets.h"
#include <string.h>
#include <stdio.h>

namespace Thriot
{
unsigned char buffer[LWS_SEND_BUFFER_PRE_PADDING + 1024 + LWS_SEND_BUFFER_POST_PADDING];

int WebSocketConnection::lws_callback(struct libwebsocket_context *context, struct libwebsocket *wsi,
              int reason, void *userdata, void *in, size_t len)
{
	WebSocketConnection *webSocketConnection = (WebSocketConnection *)userdata;
	int size;
	switch((libwebsocket_callback_reasons)reason)
	{
		case LWS_CALLBACK_CLIENT_ESTABLISHED:
			webSocketConnection->_isConnected = true;
			break;

		case LWS_CALLBACK_CLOSED:
			webSocketConnection->_isConnected = false;
			break;

		case LWS_CALLBACK_CLIENT_RECEIVE:
			webSocketConnection->_partialReceived.append((const char *)in, len);

			if(libwebsocket_is_final_fragment(wsi))
			{
				if(!webSocketConnection->_onMessage(webSocketConnection->_object, webSocketConnection->_partialReceived))
				{
					webSocketConnection->_messageReceived = webSocketConnection->_partialReceived;
				}
				webSocketConnection->_partialReceived.clear();
			}
			break;

		case LWS_CALLBACK_CLIENT_WRITEABLE:
			size = webSocketConnection->_messageToSend.size();
			if(size > 0 && size < 1024)
			{
				unsigned char *bp = &buffer[LWS_SEND_BUFFER_PRE_PADDING];
				memcpy(bp, webSocketConnection->_messageToSend.c_str(), size);

				// TODO: handle partial writes
				libwebsocket_write(wsi, bp, size, LWS_WRITE_TEXT);

				webSocketConnection->_messageToSend.clear();
			}

			break;

		default:
			break;
	}
	return 0;
}

static struct libwebsocket_protocols protocols[] = 
{
	{
		NULL,
		(callback_function*)WebSocketConnection::lws_callback,
		0
	},
	{
		NULL,
		NULL,
		0
	}
};

bool WebSocketConnection::Connect(const string& url)
{
	struct lws_context_creation_info info;
	char host[128];
	int port;
	char path[128];
	lws_set_log_level(0, NULL);
	if (url.size() >= 128)
		return false;

	memset(&info, 0, sizeof(info));

	info.port = CONTEXT_PORT_NO_LISTEN;
	info.gid = -1;
	info.uid = -1;
	info.protocols = protocols;

	_context = libwebsocket_create_context(&info);

	if(_context == NULL)
		return false;

	if (sscanf(url.c_str(), "ws://%[^:/]:%d/%s", host, &port, path) == 3) 
	{
	}
	else if (sscanf(url.c_str(), "ws://%[^:/]/%s", host, path) == 2) 
	{
		port = 80;
	}
	else if (sscanf(url.c_str(), "ws://%[^:/]:%d", host, &port) == 2) 
	{
		path[0] = '\0';
	}
	else if (sscanf(url.c_str(), "ws://%[^:/]", host) == 1) 
	{
		port = 80;
		path[0] = '\0';
	}
	else {
		return false;
    }

	_wsi = libwebsocket_client_connect_extended(_context, host, port, 0, path, host, host, NULL, 13, this);

	if(_wsi == NULL)
		return false;

	time_t startTime = time(NULL);
	while((time(NULL) - startTime) < 5 && !_isConnected)// about 5 sec
	{
		Spin();
	}
	
	return true;
}

void WebSocketConnection::OnMessage(void *object, MessagePayloadReceivedFunc onMessage)
{
	_object = object;
	_onMessage = onMessage;
}

bool WebSocketConnection::IsDisconnected()
{
	return !_isConnected;
}

void WebSocketConnection::Send(const string& message)
{
	_messageToSend = message;
	libwebsocket_callback_on_writable(_context, _wsi);

	time_t startTime = time(NULL);
	while((time(NULL) - startTime) < 2 && !_messageToSend.empty() && _isConnected) // at least 1 sec
	{
		Spin();
	}
	_messageToSend.clear();
}

string WebSocketConnection::WaitResponse(const int timeout)
{
	time_t startTime = time(NULL);
	while((time(NULL) - startTime) < timeout + 1 && _messageReceived.empty() && _isConnected)
	{
		Spin();
	}
	string response = _messageReceived;
	_messageReceived = "";

	return response;
}

void WebSocketConnection::Spin()
{
	libwebsocket_service(_context, 100);
}

void WebSocketConnection::Close()
{
	_isConnected = false;
	libwebsocket_context_destroy(_context);
}
}

