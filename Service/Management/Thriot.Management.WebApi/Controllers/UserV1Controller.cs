﻿using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Thriot.Framework.Logging;
using Thriot.Management.Dto;
using Thriot.Management.Services;
using Thriot.Management.WebApi.Auth;
using Thriot.Management.WebApi.WebFunctions;
using Thriot.Web.Models;

namespace Thriot.Management.WebApi.Controllers
{
    [RoutePrefix("v1/users")]
    public class UsersV1Controller : ApiController, IUserPrincipalContext, ILoggerOwner
    {
        private readonly UserService _userService;
        private readonly ISettingProvider _settingProvider;

        public UsersV1Controller(UserService userService, ISettingProvider settingProvider)
        {
            _userService = userService;
            _settingProvider = settingProvider;

            _userService.AuthenticationContext.SetUserPrincipalContext(this);
        }

        [Route("register")]
        [HttpPost]
        public RegistrationResultDto Register(RegisterDto register) // POST: api/v1/users/register
        {
            var needsActivation = _settingProvider.EmailActivation;

            _userService.Register(register, new Mailer());

            return new RegistrationResultDto { NeedsActivation = needsActivation };
        }

        [Route("activate/{userId}/{activationCode}")]
        [HttpGet]
        public HttpResponseMessage Activate(string userId, string activationCode)
        {
            _userService.Activate(userId, activationCode);

            var response = new HttpResponseMessage(HttpStatusCode.Redirect);
            response.Headers.Location = new Uri(_settingProvider.WebsiteUrl);
            return response;
        }

        [Route("resendActivationEmail")]
        [HttpPost]
        public HttpResponseMessage ResendActivationEmail(EmailWrapperDto emailWrapperDto)
        {
            _userService.ResendActivationEmail(emailWrapperDto.Email, new Mailer());

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        [Route("sendForgotPasswordEmail")]
        [HttpPost]
        public HttpResponseMessage SendForgotPasswordEmail(EmailWrapperDto emailWrapperDto)
        {
            _userService.SendForgotPasswordEmail(emailWrapperDto.Email, new Mailer());

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        [Route("resetPassword")]
        [HttpPost]
        public HttpResponseMessage ResetPassword(ResetPasswordDto resetPassword)
        {
            _userService.ResetPassword(resetPassword);

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        [Route("changePassword")]
        [HttpPost]
        [WebApiAuthorize]
        public HttpResponseMessage ChangePassword(ChangePasswordDto changePassword)
        {
            _userService.ChangePassword(changePassword);

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        [Route("login")]
        [HttpPost]
        public HttpResponseMessage Login(LoginDto login) // POST: api/v1/users/login
        {
            var userId = _userService.Login(login.Email, login.Password);

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        [Route("logoff")]
        [HttpPost]
        public HttpResponseMessage Logoff() // POST: api/v1/users/logoff
        {
            _userService.AuthenticationContext.RemoveContextUser();

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        [Route("me")]
        [WebApiAuthorize]
        public UserDto Get() // GET: api/v1/users/me
        {
            return _userService.GetMe();
        }

        [Route("byemail/{email}")]
        [HttpGet]
        [WebApiAuthorize]
        public UserDto FindUser([FromUri]string email) // GET: api/v1/users/email/urlencode(user@domain.com)
        {
            return _userService.FindUser(email);
        }

        private static readonly ILogger _logger = LoggerFactory.GetCurrentClassLogger();

        public ILogger Logger
        {
            get { return _logger; }
        }

        public string UserDefinedLogValue
        {
            get { return null; }
        }
    }
}