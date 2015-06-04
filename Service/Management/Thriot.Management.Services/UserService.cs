using System.Collections.Generic;
using System.Security.Authentication;
using AutoMapper;
using Thriot.Framework;
using Thriot.Framework.Exceptions;
using Thriot.Management.Dto;
using Thriot.Management.Model;
using Thriot.Management.Model.Exceptions;
using Thriot.Management.Model.Operations;

namespace Thriot.Management.Services
{
    public class UserService
    {
        private readonly IUserOperations _userOperations;
        private readonly IAuthenticationContext _authenticationContext;
        private readonly ISettingProvider _settingProvider;
        private readonly IEnvironmentPrebuilder _environmentPrebuilder;

        public UserService(IUserOperations userOperations, IAuthenticationContext authenticationContext, ISettingProvider settingProvider, IEnvironmentPrebuilder environmentPrebuilder)
        {
            _userOperations = userOperations;
            _authenticationContext = authenticationContext;
            _settingProvider = settingProvider;
            _environmentPrebuilder = environmentPrebuilder;
        }

        public IAuthenticationContext AuthenticationContext
        {
            get { return _authenticationContext; }
        }

        public string Register(RegisterDto register, IMailer mailer)
        {
            register.Email = Validator.ValidateEmail(register.Email);
            register.Name = Validator.TrimAndValidateAsName(register.Name);
            Validator.ValidatePassword(register.Password);

            var user = Mapper.Map<RegisterDto, User>(register);

            user.Email = user.Email.ToLower();

            if (_userOperations.IsExists(user.Email))
                throw new AlreadyExistsException();

            var needsActivation = _settingProvider.EmailActivation;

            if (!needsActivation)
            {
                user.Activated = true;
            }
            else
            {
                user.Activated = false;
                user.ActivationCode = Identity.Next();
            }

            var salt = Crypto.GenerateSalt();
            var passwordHash = Crypto.CalcualteHash(register.Password, salt);

            var userId = _userOperations.Create(user, passwordHash, salt);

            if (needsActivation)
            {
                mailer.SendActivationMail(userId, user.Name, user.Email, user.ActivationCode, _settingProvider.WebsiteUrl);
            }
            else
            {
                _authenticationContext.SetContextUser(userId);

                EnsureEnvironment();
            }

            return userId;
        }

        public string Login(string email, string password)
        {
            LoginUser loginUser = null;
            try
            {
                loginUser = _userOperations.GetLoginUser(email.ToLower());
            }
            catch (NotFoundException)
            {
                throw new AuthenticationException();
            }

            var rehash = Crypto.CalcualteHash(password, loginUser.Salt);

            if (rehash != loginUser.PasswordHash)
                throw new AuthenticationException();

            var userId = loginUser.UserId;

            var user = _userOperations.Get(userId);

            if(!user.Activated)
                throw new ActivationRequiredException();

            _authenticationContext.SetContextUser(userId);

            EnsureEnvironment();

            return userId;
        }

        public void Activate(string userId, string activationCode)
        {
            Validator.ValidateId(userId);
            Validator.ValidateId(activationCode);

            EnsureLoggedOff();

            var user = _userOperations.Get(userId);

            if (user.Activated)
                throw new ActivationException("Already activated");

            if (user.ActivationCode != activationCode)
                throw new ActivationException("Unable to activate user");

            user.Activated = true;
            user.ActivationCode = Identity.Next();
            _userOperations.Update(user);

            _authenticationContext.SetContextUser(userId);

            EnsureEnvironment();
        }

        public void ChangePassword(ChangePasswordDto changePassword)
        {
            var currentUserId = GetCurrentUser();

            Validator.ValidatePassword(changePassword.NewPassword);

            var user = _userOperations.Get(currentUserId);
            var loginUser = _userOperations.GetLoginUser(user.Email);

            var hash = Crypto.CalcualteHash(changePassword.CurrentPassword, loginUser.Salt);
            if (hash != loginUser.PasswordHash)
                throw new AuthenticationException("Invalid old password");

            var salt = Crypto.GenerateSalt();
            var passwordHash = Crypto.CalcualteHash(changePassword.NewPassword, salt);

            _userOperations.Update(new LoginUser
            {
                Email = user.Email,
                PasswordHash = passwordHash,
                Salt = salt,
                UserId = user.Id
            });
        }

        public void ResendActivationEmail(string email, IMailer mailer)
        {
            EnsureLoggedOff();

            var targetEmail = Validator.ValidateEmail(email).ToLower();
            var loginUser = _userOperations.GetLoginUser(targetEmail);

            var user = _userOperations.Get(loginUser.UserId);
            if (user.Activated)
                throw new ActivationException("User already activated");

            mailer.SendActivationMail(user.Id, user.Name, user.Email, user.ActivationCode, _settingProvider.ManagementApiUrl);
        }

        public void SendForgotPasswordEmail(string email, IMailer mailer)
        {
            EnsureLoggedOff();

            var targetEmail = Validator.ValidateEmail(email).ToLower();
            var loginUser = _userOperations.GetLoginUser(targetEmail);

            var user = _userOperations.Get(loginUser.UserId);
            if (!user.Activated)
                throw new ActivationException("Please activate the user first");

            user.ActivationCode = Identity.Next();
            _userOperations.Update(user);

            mailer.SendForgotPasswordEmail(user.Id, user.Name, user.Email, user.ActivationCode, _settingProvider.WebsiteUrl);
        }

        public void ResetPassword(ResetPasswordDto resetPassword)
        {
            EnsureLoggedOff();

            Validator.ValidatePassword(resetPassword.Password);

            var user = _userOperations.Get(resetPassword.UserId);
            if (!user.Activated)
                throw new ActivationException("Please activate the user first");

            if (user.ActivationCode != resetPassword.ConfirmationCode)
                throw new ConfirmationException("Unable to reset password due to bad confirmation code");

            user.ActivationCode = Identity.Next();

            var salt = Crypto.GenerateSalt();
            var passwordHash = Crypto.CalcualteHash(resetPassword.Password, salt);

            _userOperations.Update(new LoginUser
            {
                Email = user.Email,
                PasswordHash = passwordHash,
                Salt = salt,
                UserId = user.Id
            });
            _userOperations.Update(user);
        }

        public IList<SmallDto> ListCompanies()
        {
            var userId = GetCurrentUser();

            var companies = _userOperations.ListCompanies(userId);
            return Mapper.Map<IList<Small>, IList<SmallDto>>(companies);
        }

        public UserDto GetMe()
        {
            var userId = GetCurrentUser();

            var user = _userOperations.Get(userId);

            return Mapper.Map<UserDto>(user);
        }

        public UserDto FindUser(string email)
        {
            if (_authenticationContext.GetContextUser() == null)
                throw new AuthenticationException();

            email = Validator.ValidateEmail(email).ToLower();

            try
            {
                var loginUser = _userOperations.GetLoginUser(email);

                var user = _userOperations.Get(loginUser.UserId);
                return Mapper.Map<UserDto>(user);
            }
            catch (NotFoundException)
            {
                return null;
            }
        }

        private string GetCurrentUser()
        {
            var userId = _authenticationContext.GetContextUser();
            if (userId == null)
                throw new AuthenticationException();
            return userId;
        }

        private void EnsureLoggedOff()
        {
            var currentUserId = _authenticationContext.GetContextUser();
            if (currentUserId != null)
                throw new AuthenticationException("Please logoff");
        }

        private void EnsureEnvironment()
        {
            if (_settingProvider.ServiceProfile == ServiceProfile.SingleCompany ||
                _settingProvider.ServiceProfile == ServiceProfile.SingleService)
            {
                _environmentPrebuilder.EnsureEnvironment();
            }
        }
    }
}
