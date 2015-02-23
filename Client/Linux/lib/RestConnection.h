#pragma once

#include <string>
#include <map>

using namespace std;

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
		bool _isAuthenticated;
		map<string, string> _requestHeaders;

	public:
		RestConnection(string baseUrl);

		void ClearAuthToken();
		void SetAuthToken(string authToken);
		bool IsAuthenticated();

		void ClearRequestHeaders();
		void AddRequestHeader(string key, string value);

		Response Get(const string& url);
		Response Post(const string& url, const string& contentType, const string& data);
		Response Put(const string& url, const string& contentType, const string& data);
		Response Delete(const string& url);
};

