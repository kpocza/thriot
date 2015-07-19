using System;
using Microsoft.AspNet.Mvc;
using Thriot.Framework.Logging;
using Thriot.Management.Dto;
using Thriot.Management.Services;
using Thriot.Management.WebApi.Auth;
using Thriot.Management.WebApi.WebFunctions;
using Thriot.Web.Models;

namespace Thriot.Management.WebApi.Controllers
{
    [Route("v1/users")]
    public class UsersV1Controller : Controller, ILoggerOwner
    {
        private readonly UserService _userService;
        private readonly ISettingProvider _settingProvider;

        public UsersV1Controller(UserService userService, ISettingProvider settingProvider)
        {
            _userService = userService;
            _settingProvider = settingProvider;
        }

        [HttpPost("register")]
        public RegistrationResultDto Register([FromBody]RegisterDto register) // POST: api/v1/users/register
        {
            var needsActivation = _settingProvider.EmailActivation;

            _userService.Register(register, new Mailer());

            return new RegistrationResultDto { NeedsActivation = needsActivation };
        }

        [HttpGet("activate/{userId}/{activationCode}")]
        public IActionResult Activate(string userId, string activationCode)
        {
            _userService.Activate(userId, activationCode);

            return new NoContentResult();
        }

        [HttpPost("resendActivationEmail")]
        public IActionResult ResendActivationEmail([FromBody]EmailWrapperDto emailWrapperDto)
        {
            _userService.ResendActivationEmail(emailWrapperDto.Email, new Mailer());

            return new NoContentResult();
        }

        [HttpPost("sendForgotPasswordEmail")]
        public IActionResult SendForgotPasswordEmail([FromBody]EmailWrapperDto emailWrapperDto)
        {
            _userService.SendForgotPasswordEmail(emailWrapperDto.Email, new Mailer());

            return new NoContentResult();
        }

        [HttpPost("resetPassword")]
        public IActionResult ResetPassword([FromBody]ResetPasswordDto resetPassword)
        {
            _userService.ResetPassword(resetPassword);

            return new NoContentResult();
        }

        [HttpPost("changePassword")]
        [WebApiAuthorize]
        public IActionResult ChangePassword([FromBody]ChangePasswordDto changePassword)
        {
            _userService.ChangePassword(changePassword);

            return new NoContentResult();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody]LoginDto login) // POST: api/v1/users/login
        {
            var userId = _userService.Login(login.Email, login.Password);

            return new NoContentResult();
        }

        [HttpPost("logoff")]
        public IActionResult Logoff() // POST: api/v1/users/logoff
        {
            _userService.AuthenticationContext.RemoveContextUser();

            return new NoContentResult();
        }

        [HttpGet("me")]
        [WebApiAuthorize]
        public UserDto Get() // GET: api/v1/users/me
        {
            return _userService.GetMe();
        }

        [HttpGet("byemail/{email}/")]
        [WebApiAuthorize]
        public UserDto FindUser(string email) // GET: api/v1/users/byemail/urlencode(user@domain.com)
        {
            // Quick and dirty workaround for mono
            // TODO: check later on kestrel whether it's still needed
            if (Thriot.Framework.Environment.IsMono())
            {
                email = Uri.UnescapeDataString(email);
            }

            return _userService.FindUser(email);
        }

        private static readonly ILogger _logger = LoggerFactory.GetCurrentClassLogger();

        public ILogger Logger
        {
            get { return _logger; }
        }

        public string UserDefinedLogValue
        {
            get { return Context?.User?.Identity?.Name; }
        }
    }
}
