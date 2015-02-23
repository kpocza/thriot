using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using IoT.Client.DotNet.Management.Dto;

namespace IoT.Client.DotNet.Management
{
    public class UserManagementClient : SpecificManagementClient
    {
        private readonly string _baseUrl;
        private bool _isLoggedIn;

        internal UserManagementClient(string baseUrl, IRestConnection restConnection) : base(restConnection)
        {
            _baseUrl = baseUrl;
            _isLoggedIn = false;
        }

        public void Register(Register register)
        {
            var response = RestConnection.Post("users/register", JsonSerializer.Serialize(register));

            var registrationResult = JsonSerializer.Deserialize<RegistrationResult>(response);

            if (registrationResult.NeedsActivation)
                throw new Exception(
                    "Activation needed, please confirm your account by clicking the email you received by email");

            RestConnection.Setup(_baseUrl,
                new Dictionary<string, string>
                {
                    {
                        HttpRequestHeader.Authorization.ToString(), "Basic " + registrationResult.AuthToken
                    }
                });
            _isLoggedIn = true;
        }

        public User Get()
        {
            var userStr = RestConnection.Get("users/me");

            return JsonSerializer.Deserialize<User>(userStr);
        }

        public User FindUser(string email)
        {
            var userStr = RestConnection.Get("users/byemail/" + HttpUtility.UrlEncode(email) + "/");

            return JsonSerializer.Deserialize<User>(userStr);
        }

        public void Login(Login login)
        {
            var response = RestConnection.Post("users/login", JsonSerializer.Serialize(login));

            RestConnection.Setup(_baseUrl,
                new Dictionary<string, string>
                {
                    {
                        HttpRequestHeader.Authorization.ToString(), "Basic " + JsonSerializer.Deserialize<string>(response)
                    }
                });
            _isLoggedIn = true;
        }

        public void Logoff()
        {
            RestConnection.Setup(_baseUrl, null);
            _isLoggedIn = false;
        }

        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
        }

    }
}
