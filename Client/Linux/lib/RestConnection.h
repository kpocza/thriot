#pragma once

#include <string>
#include <map>

using namespace std;

namespace Thriot
{
struct Response
{
	int Code;
	string Body;
	map<string, string> Headers;
};

class RestConnection
{
	private:
		string _baseUrl;
		map<string, string> _requestHeaders;
		void *_curl;

	public:
		RestConnection(string baseUrl);
		~RestConnection();

		void EnableCookies();
		void ClearRequestHeaders();
		void AddRequestHeader(string key, string value);

		Response Get(const string& url);
		Response Post(const string& url, const string& contentType, const string& data);
		Response Put(const string& url, const string& contentType, const string& data);
		Response Delete(const string& url);
};
}

