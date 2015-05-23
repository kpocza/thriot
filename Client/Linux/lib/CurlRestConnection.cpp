#include "RestConnection.h"
#include "base64.h"
#include <curl/curl.h>
#include <cstdlib>
#include <string.h>
#include <algorithm>

#include <iostream>
using namespace std;

namespace Thriot
{
#define USERAGENT "Thriot Linux Curl"

typedef struct
{
	const char* data;
	size_t length;
} upload_object;

size_t write_callback(void *data, size_t size, size_t nmemb, void *userdata);
size_t header_callback(void *data, size_t size, size_t nmemb, void *userdata);
size_t read_callback(void *data, size_t size, size_t nmemb, void *userdata);

RestConnection::RestConnection(string baseUrl)
{
	_baseUrl = baseUrl;
	_curl = curl_easy_init();
	curl_easy_setopt(_curl, CURLOPT_VERBOSE, true);
}

RestConnection::~RestConnection()
{
	curl_easy_cleanup(_curl);
}

void RestConnection::EnableCookies()
{
	curl_easy_setopt(_curl, CURLOPT_COOKIEFILE, "");
}

void RestConnection::AddRequestHeader(string key, string value)
{
	_requestHeaders[key] = value;
}

void RestConnection::ClearRequestHeaders()
{
	_requestHeaders.clear();
}

Response RestConnection::Get(const string& url)
{
	Response ret;

	CURLcode res = CURLE_OK;

	curl_easy_reset(_curl);
	curl_easy_setopt(_curl, CURLOPT_USERAGENT, USERAGENT);
	curl_easy_setopt(_curl, CURLOPT_URL, (_baseUrl + "/" + url).c_str());
	curl_easy_setopt(_curl, CURLOPT_WRITEFUNCTION, write_callback);
	curl_easy_setopt(_curl, CURLOPT_WRITEDATA, &ret);
	curl_easy_setopt(_curl, CURLOPT_HEADERFUNCTION, header_callback);
	curl_easy_setopt(_curl, CURLOPT_HEADERDATA, &ret);
		
	curl_slist* header = NULL;
	for(map<string, string>::const_iterator it = _requestHeaders.begin(); it!= _requestHeaders.end();++it)
	{
		header = curl_slist_append(header, (it->first + ": " + it->second).c_str());
	}
	curl_easy_setopt(_curl, CURLOPT_HTTPHEADER, header);

	res = curl_easy_perform(_curl);
	if (res != CURLE_OK)
	{
		ret.Code = -1;
		return ret;
	}
	long http_code = 0;
	curl_easy_getinfo(_curl, CURLINFO_RESPONSE_CODE, &http_code);
	ret.Code = static_cast<int>(http_code);

	curl_slist_free_all(header);

	return ret;
}

Response RestConnection::Post(const string& url, const string& contentType, const string& data)
{
	Response ret;

	CURLcode res = CURLE_OK;

	curl_easy_reset(_curl);
	curl_easy_setopt(_curl, CURLOPT_USERAGENT, USERAGENT);
	curl_easy_setopt(_curl, CURLOPT_URL, (_baseUrl + "/" + url).c_str());
	curl_easy_setopt(_curl, CURLOPT_POST, 1L);
	curl_easy_setopt(_curl, CURLOPT_POSTFIELDS, data.c_str());
	curl_easy_setopt(_curl, CURLOPT_POSTFIELDSIZE, data.size());
	curl_easy_setopt(_curl, CURLOPT_WRITEFUNCTION, write_callback);
	curl_easy_setopt(_curl, CURLOPT_WRITEDATA, &ret);
	curl_easy_setopt(_curl, CURLOPT_HEADERFUNCTION, header_callback);
	curl_easy_setopt(_curl, CURLOPT_HEADERDATA, &ret);

	curl_slist* header = NULL;
	header = curl_slist_append(header, ("Content-Type: " + contentType).c_str());
	for(map<string, string>::const_iterator it = _requestHeaders.begin(); it!= _requestHeaders.end();++it)
	{
		header = curl_slist_append(header, (it->first + ": " + it->second).c_str());
	}
	curl_easy_setopt(_curl, CURLOPT_HTTPHEADER, header);

	res = curl_easy_perform(_curl);
	if (res != CURLE_OK)
	{
		ret.Code = -1;
		return ret;
	}
	long http_code = 0;
	curl_easy_getinfo(_curl, CURLINFO_RESPONSE_CODE, &http_code);
	ret.Code = static_cast<int>(http_code);

	curl_slist_free_all(header);

	return ret;
}

Response RestConnection::Put(const string& url, const string& contentType, const string& data)
{
	Response ret;

	CURLcode res = CURLE_OK;
	upload_object up_obj;
	up_obj.data = data.c_str();
	up_obj.length = data.size();

	curl_easy_reset(_curl);
	curl_easy_setopt(_curl, CURLOPT_USERAGENT, USERAGENT);
	curl_easy_setopt(_curl, CURLOPT_URL, (_baseUrl + "/" + url).c_str());
	curl_easy_setopt(_curl, CURLOPT_PUT, 1L);
	curl_easy_setopt(_curl, CURLOPT_UPLOAD, 1L);
	curl_easy_setopt(_curl, CURLOPT_READFUNCTION, read_callback);
	curl_easy_setopt(_curl, CURLOPT_READDATA, &up_obj);
 	curl_easy_setopt(_curl, CURLOPT_WRITEFUNCTION, write_callback);
	curl_easy_setopt(_curl, CURLOPT_WRITEDATA, &ret);
	curl_easy_setopt(_curl, CURLOPT_HEADERFUNCTION, header_callback);
	curl_easy_setopt(_curl, CURLOPT_HEADERDATA, &ret);
	curl_easy_setopt(_curl, CURLOPT_INFILESIZE, static_cast<long>(up_obj.length));

	curl_slist* header = NULL;
	header = curl_slist_append(header, ("Content-Type: " + contentType).c_str());
	for(map<string, string>::const_iterator it = _requestHeaders.begin(); it!= _requestHeaders.end();++it)
	{
		header = curl_slist_append(header, (it->first + ": " + it->second).c_str());
	}
	curl_easy_setopt(_curl, CURLOPT_HTTPHEADER, header);

	res = curl_easy_perform(_curl);
	if (res != CURLE_OK)
	{
		ret.Code = -1;
		return ret;
	}
	long http_code = 0;
	curl_easy_getinfo(_curl, CURLINFO_RESPONSE_CODE, &http_code);
	ret.Code = static_cast<int>(http_code);

	curl_slist_free_all(header);

	return ret;
}

Response RestConnection::Delete(const string& url)
{
	Response ret;

	CURLcode res = CURLE_OK;

	curl_easy_reset(_curl);
	curl_easy_setopt(_curl, CURLOPT_USERAGENT, USERAGENT);
	curl_easy_setopt(_curl, CURLOPT_URL, (_baseUrl + "/" + url).c_str());
	curl_easy_setopt(_curl, CURLOPT_CUSTOMREQUEST, "DELETE");
	curl_easy_setopt(_curl, CURLOPT_WRITEFUNCTION, write_callback);
	curl_easy_setopt(_curl, CURLOPT_WRITEDATA, &ret);
	curl_easy_setopt(_curl, CURLOPT_HEADERFUNCTION, header_callback);
	curl_easy_setopt(_curl, CURLOPT_HEADERDATA, &ret);
		
	curl_slist* header = NULL;
	for(map<string, string>::const_iterator it = _requestHeaders.begin(); it!= _requestHeaders.end();++it)
	{
		header = curl_slist_append(header, (it->first + ": " + it->second).c_str());
	}
	curl_easy_setopt(_curl, CURLOPT_HTTPHEADER, header);

	res = curl_easy_perform(_curl);
	if (res != CURLE_OK)
	{
		ret.Code = -1;
		return ret;
	}
	long http_code = 0;
	curl_easy_getinfo(_curl, CURLINFO_RESPONSE_CODE, &http_code);
	ret.Code = static_cast<int>(http_code);

	curl_slist_free_all(header);

	return ret;
}

static inline string &ltrim(string &s) {
	s.erase(s.begin(), find_if(s.begin(), s.end(), not1(std::ptr_fun<int, int>(isspace))));
	return s;
}
		
static inline string &rtrim(string &s) {
	s.erase(find_if(s.rbegin(), s.rend(), not1(ptr_fun<int, int>(isspace))).base(), s.end());
	return s;
}

static inline string &trim(string &s) {
	return ltrim(rtrim(s));
}

size_t write_callback(void *data, size_t size, size_t nmemb, void *userdata)
{
	Response* r = reinterpret_cast<Response*>(userdata);
	r->Body.append(reinterpret_cast<char*>(data), size*nmemb);

	return (size * nmemb);
}

size_t header_callback(void *data, size_t size, size_t nmemb, void *userdata)
{
	Response* r = reinterpret_cast<Response*>(userdata);
	string header(reinterpret_cast<char*>(data), size*nmemb);
	size_t seperator = header.find_first_of(":");
	if (string::npos == seperator) 
	{
		trim(header);
		if(0 == header.length())
		{
			return (size * nmemb);
		}

		r->Headers[header] = "";
	}
	else 
	{
		string key = header.substr(0, seperator);
		trim(key);
		string value = header.substr(seperator + 1);
		trim (value);
		r->Headers[key] = value;
	}

	return (size * nmemb);
}

size_t read_callback(void *data, size_t size, size_t nmemb, void *userdata)
{
	upload_object* u = reinterpret_cast<upload_object*>(userdata);
	size_t curl_size = size * nmemb;
	size_t copy_size = (u->length < curl_size) ? u->length : curl_size;
	memcpy(data, u->data, copy_size);
	u->length -= copy_size;
	u->data += copy_size;
	return copy_size;
}
}

