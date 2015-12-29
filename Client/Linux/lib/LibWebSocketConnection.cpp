#include "WebSocketConnection.h"
#include <string.h>
#include <stdio.h>
#include <openssl/ssl.h>

namespace Thriot
{
static int always_accept_callback(X509_STORE_CTX *ctx, void *arg)
{
	return 1;
}

unsigned char buffer[LWS_SEND_BUFFER_PRE_PADDING + 1024 + LWS_SEND_BUFFER_POST_PADDING];

lws_context *WebSocketConnection::_context = NULL;

int WebSocketConnection::lws_callback(struct lws *wsi, enum lws_callback_reasons reason,
	void *userdata, void *in, size_t len)
{
	WebSocketConnection *webSocketConnection = (WebSocketConnection *)userdata;
	int size;

	switch(reason)
	{
		case LWS_CALLBACK_OPENSSL_LOAD_EXTRA_CLIENT_VERIFY_CERTS:
			SSL_CTX_set_cert_verify_callback((SSL_CTX*)userdata, always_accept_callback, NULL);
			break;
		case LWS_CALLBACK_CLIENT_ESTABLISHED:
			webSocketConnection->_isConnected = true;
			break;

		case LWS_CALLBACK_CLOSED:
			webSocketConnection->_isConnected = false;
			break;

		case LWS_CALLBACK_CLIENT_RECEIVE:
			webSocketConnection->_partialReceived.append((const char *)in, len);

			if(lws_is_final_fragment(wsi))
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
				lws_write(wsi, bp, size, LWS_WRITE_TEXT);

				webSocketConnection->_messageToSend.clear();
			}

			break;

		default:
			break;
	}
	return 0;
}

static struct lws_protocols protocols[] = 
{
	{
		NULL,
		WebSocketConnection::lws_callback,
		0
	},
	{
		NULL,
		NULL,
		0
	}
};


//void logger(int level, const char *line){std::cout << line << std::endl;}

bool WebSocketConnection::Connect(const string& url)
{
	struct lws_context_creation_info info;
	char host[128];
	int port;
	char path[128];
	bool isSecure = false;
	int connectionType = 0;

	if(WebSocketConnection::_context == NULL)
	{
//		uncomment for more detailed logging
//		lws_set_log_level(7, logger);

		lws_set_log_level(0, NULL);

		memset(&info, 0, sizeof(info));

		info.port = CONTEXT_PORT_NO_LISTEN;
		info.gid = -1;
		info.uid = -1;
		info.protocols = protocols;

		WebSocketConnection::_context = lws_create_context(&info);
	}

	if(WebSocketConnection::_context == NULL)
		return false;

	if (url.size() >= 128)
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
	else if (sscanf(url.c_str(), "wss://%[^:/]:%d/%s", host, &port, path) == 3) 
	{
		isSecure = true;
	}
	else if (sscanf(url.c_str(), "wss://%[^:/]/%s", host, path) == 2) 
	{
		isSecure = true;
		port = 443;
	}
	else if (sscanf(url.c_str(), "wss://%[^:/]:%d", host, &port) == 2) 
	{
		isSecure = true;
		path[0] = '\0';
	}
	else if (sscanf(url.c_str(), "wss://%[^:/]", host) == 1) 
	{
		isSecure = true;
		port = 443;
		path[0] = '\0';
	}
	else {
		return false;
    	}

	connectionType = 0;
	if(isSecure) {
		connectionType = 2;
	}
	_wsi = lws_client_connect_extended(WebSocketConnection::_context, host, port, connectionType, path, host, host, NULL, 13, this);

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
	lws_callback_on_writable(_wsi);

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
	lws_service(WebSocketConnection::_context, 100);
}

void WebSocketConnection::Close()
{
	_isConnected = false;
}
}

