using System.Collections.Generic;
using System.Security.Authentication;
using AutoMapper;
using IoT.Framework;
using IoT.Framework.Exceptions;
using IoT.Management.Dto;
using IoT.Management.Model.Exceptions;
using IoT.Management.Model.Operations;
using IoT.Management.Model;

namespace IoT.Management.Services
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

        public string Register(RegisterDto register, string password, IMailer mailer)
        {
            register.Email = Validator.ValidateEmail(register.Email);
            register.Name = Validator.TrimAndValidateAsName(register.Name);
            Validator.ValidatePassword(password);

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
            var passwordHash = Crypto.CalcualteHash(password, salt);

            var userId = _userOperations.Create(user, passwordHash, salt);

            if (needsActivation)
            {
                mailer.SendActivationMail(userId, user.Name, user.Email, user.ActivationCode, _settingProvider.ManagementApiUrl);
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

            var currentUserId = _authenticationContext.GetContextUser();
            if (currentUserId != null)
                throw new ActivationException("Please logoff");

            var user = _userOperations.Get(userId);

            if (user.Activated)
                throw new ActivationException("Already activated");

            if (user.ActivationCode != activationCode)
                throw new ActivationException("Unable to activate user");

            user.Activated = true;
            _userOperations.Update(user);

            _authenticationContext.SetContextUser(userId);

            EnsureEnvironment();
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
