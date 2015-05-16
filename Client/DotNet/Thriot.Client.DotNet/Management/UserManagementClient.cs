using System.Collections.Generic;
using System.Net;
using System.Web;

namespace Thriot.Client.DotNet.Management
{
    /// <summary>
    /// This class is responsible for managing users and doing login operations. Activation is not currently the part of this library.
    /// </summary>
    public class UserManagementClient : SpecificManagementClient
    {
        private readonly string _baseUrl;
        private bool _isLoggedIn;

        internal UserManagementClient(string baseUrl, IRestConnection restConnection) : base(restConnection)
        {
            _baseUrl = baseUrl;
            _isLoggedIn = false;
        }

        /// <summary>
        /// Register a new user in the system. At minimum the email address, the full name and the password is required at registration.
        /// If the system is configured not to require user activation after registration the function also logs in the user just registered.
        /// However if activation is required the <see cref="ActivationRequiredException" /> exeption will be thrown with the following message
        /// "Activation needed, please confirm your account by clicking the link you received by email" but the user will be registered. 
        /// It meens that the user registration happened but the system cannot be used unless clicking the activation link.
        /// 
        /// Send POST request to APIROOT/users/register
        /// </summary>
        /// <param name="register">Registration parameters</param>
        /// <exception cref="ActivationRequiredException">This exception indicates that the registration succeeded but activation is required before using the system</exception>
        /// <exception cref="WebException">This exception will be thrown in case of service side registration error. Please refer to the HTTP error code for more information</exception>
        public void Register(Register register)
        {
            var response = RestConnection.Post("users/register", JsonSerializer.Serialize(register));

            var registrationResult = JsonSerializer.Deserialize<RegistrationResult>(response);

            if (registrationResult.NeedsActivation)
                throw new ActivationRequiredException(
                    "Activation needed, please confirm your account by clicking the link you received by email");

            RestConnection.Setup(_baseUrl,
                new Dictionary<string, string>
                {
                    {
                        HttpRequestHeader.Authorization.ToString(), "Basic " + registrationResult.AuthToken
                    }
                });
            _isLoggedIn = true;
        }

        /// <summary>
        /// Log in the current user. On successfull login all subsequent management operations will run in the context of the logged in user.
        /// 
        /// Send POST request to APIROOT/users/login
        /// </summary>
        /// <param name="login">The user's email address and password</param>
        /// <exception cref="WebException">In case of any service side error an exception will be thrown. Please refer to the HTTP error code for more information</exception>
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

        /// <summary>
        /// Activate the given user with the activation code
        /// 
        /// Send GET request to APIROOT/users/activate/{userId}/{activationCode}
        /// </summary>
        /// <param name="activate">Identifier of the user secured by the activation code to activate</param>
        public void Activate(Activate activate)
        {
            RestConnection.Get(string.Format("users/activate/{0}/{1}", activate.UserId, activate.ActivationCode));
        }

        /// <summary>
        /// Resend activation email for the given email address
        /// 
        /// Send POST request to APIROOT/users/resendActivationEmail
        /// </summary>
        /// <param name="email">Email address to send activation email for</param>
        public void ResendActivationEmail(EmailWrapper email)
        {
            RestConnection.Post("users/resendActivationEmail",
                JsonSerializer.Serialize(email));
        }

        /// <summary>
        /// Send email about forgotten password to the user having the email address in the parameter
        /// 
        /// Send POST request to APIROOT/users/sendForgotPasswordEmail
        /// </summary>
        /// <param name="email">Email address to send forgot password email for</param>
        public void SendForgotPasswordEmail(EmailWrapper email)
        {
            RestConnection.Post("users/sendForgotPasswordEmail",
                JsonSerializer.Serialize(email));
        }

        /// <summary>
        /// Reset the password specifid by the resetPassword dto. It will contain the userid, the confirmation code and the new password
        /// The first two parameters are sent out by the sendForgotPasswordEmail operation
        /// 
        /// Send POST request to APIROOT/users/resetPassword
        /// </summary>
        /// <param name="resetPassword">Parameters for resetting password</param>
        public void ResetPassword(ResetPassword resetPassword)
        {
            RestConnection.Post("users/resetPassword",
                JsonSerializer.Serialize(resetPassword));
        }

        /// <summary>
        /// Change password of the currently logged in
        /// You must specify the current and the new passwords
        /// 
        /// Send POST request to APIROOT/users/changePassword
        /// </summary>
        /// <param name="changePassword">Change password parameters</param>
        public void ChangePassword(ChangePassword changePassword)
        {
            RestConnection.Post("users/changePassword",
                JsonSerializer.Serialize(changePassword));
        }

        /// <summary>
        /// Query the currently logged in user
        /// 
        /// Send GET request to APIROOT/users/me
        /// </summary>
        /// <returns>Current user properties like full name and email address</returns>
        /// <exception cref="WebException">In case of any service side error an exception will be thrown. Please refer to the HTTP error code for more information</exception>
        public User Get()
        {
            var userStr = RestConnection.Get("users/me");

            return JsonSerializer.Deserialize<User>(userStr);
        }

        /// <summary>
        /// Find an user by email address. Only exact matches will work while the comparison is implemented case insensitively on the service side.
        /// 
        /// Send GET request to APIROOT/users/byemail/UrlEncode(email)/
        /// </summary>
        /// <param name="email">Email address to look for</param>
        /// <returns>User entity if the email address matches otherwise a null will be returned.</returns>
        /// <exception cref="WebException">In case of any service side error an exception will be thrown. Please refer to the HTTP error code for more information</exception>
        public User FindUser(string email)
        {
            var userStr = RestConnection.Get("users/byemail/" + HttpUtility.UrlEncode(email) + "/");

            return JsonSerializer.Deserialize<User>(userStr);
        }

        /// <summary>
        /// Logs off the user. Any subsequent management operations will fail that require authentication.
        /// </summary>
        public void Logoff()
        {
            RestConnection.Setup(_baseUrl, null);
            _isLoggedIn = false;
        }

        /// <summary>
        /// Indicates if anybody is logged in.
        /// </summary>
        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
        }
    }
}
