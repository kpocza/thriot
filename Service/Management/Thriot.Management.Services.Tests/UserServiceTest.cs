using System.Security.Authentication;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Thriot.Framework;
using Thriot.Framework.Exceptions;
using Thriot.Management.Dto;
using Thriot.Management.Model;
using Thriot.Management.Model.Exceptions;
using Thriot.TestHelpers;

namespace Thriot.Management.Services.Tests
{
    [TestClass]
    public class UserServiceTest
    {
        #region Register

        [TestMethod]
        public void RegisterTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var email = EmailHelper.Generate();

            var userId = userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", null);

            Assert.AreEqual(32, userId.Length);
            Assert.IsNull(settingProvider.UserForPrebuiltEntity);
            Assert.IsNull(settingProvider.PrebuiltCompany);
            Assert.IsNull(settingProvider.PrebuiltService);
        }

        [TestMethod]
        public void RegisterActivationNeededTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();
            var mailer = Substitute.For<IMailer>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var settingProvider = Substitute.For<ISettingProvider>();
            settingProvider.EmailActivation.Returns(true);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var email = EmailHelper.Generate();

            var userId = userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", mailer);

            Assert.AreEqual(32, userId.Length);
            mailer.ReceivedWithAnyArgs().SendActivationMail(null, null, null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(AlreadyExistsException))]
        public void TryRegisterTwiceTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var email = EmailHelper.Generate();

            userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", null);
            userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", null);
        }

        [TestMethod]
        [ExpectedException(typeof(AlreadyExistsException))]
        public void TryRegisterTwiceWithDifferentCasingTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var email = EmailHelper.Generate();

            userService.Register(new RegisterDto() { Name = "new user", Email = email.ToLower() }, "password", null);
            userService.Register(new RegisterDto() { Name = "new user", Email = email.ToUpper() }, "password", null);
        }

        #endregion

        #region Login
        [TestMethod]
        public void LoginTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var email = EmailHelper.Generate();

            var userId = userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", null);

            var loggedInUser = userService.Login(email, "password");

            Assert.AreEqual(userId, loggedInUser);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void LoginFailedTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var email = EmailHelper.Generate();

            userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", null);

            userService.Login(email, "password2");
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void LoginFailed2Test()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();

            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);
            var userOperations = environmentFactory.MgmtUserOperations;

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            userService.Login("asdfasdfg@asdfasdf.hu", "password2");
        }

        [TestMethod]
        [ExpectedException(typeof(ActivationRequiredException))]
        public void TryLoginWoActivationTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();
            var mailer = Substitute.For<IMailer>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var settingProvider = Substitute.For<ISettingProvider>();
            settingProvider.EmailActivation.Returns(true);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var email = EmailHelper.Generate();

            userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", mailer);

            userService.Login(email, "password");
        }

        #endregion

        #region Activation

        [TestMethod]
        public void ActivationTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var mailer = Substitute.For<IMailer>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();
            authenticationContext.GetContextUser().Returns((string)null);
            var userOperations = environmentFactory.MgmtUserOperations;

            var settingProvider = Substitute.For<ISettingProvider>();
            settingProvider.EmailActivation.Returns(true);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var email = EmailHelper.Generate();
            string activationCode = null;

            mailer.When(
                m => m.SendActivationMail(Arg.Any<string>(), "new user", email, Arg.Any<string>(), Arg.Any<string>()))
                .Do(call => { activationCode = (string)call.Args()[3]; });

            var userId = userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", mailer);

            userService.Activate(userId, activationCode);

            authenticationContext.GetContextUser().Returns(userId);

            userService.Login(email, "password");
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void ActivationAlreadyLoggedInFailTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var mailer = Substitute.For<IMailer>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();
            authenticationContext.GetContextUser().Returns("123456");
            var userOperations = environmentFactory.MgmtUserOperations;
            var settingProvider = Substitute.For<ISettingProvider>();
            settingProvider.EmailActivation.Returns(true);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var email = EmailHelper.Generate();
            string activationCode = null;

            mailer.When(
                m => m.SendActivationMail(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()))
                .Do(call => { activationCode = (string)call.Args()[3]; });
            var userId = userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", mailer);

            userService.Activate(userId, activationCode);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void ActivationInvalidUserFailTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var mailer = Substitute.For<IMailer>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();
            authenticationContext.GetContextUser().Returns((string)null);
            var userOperations = environmentFactory.MgmtUserOperations;
            var settingProvider = Substitute.For<ISettingProvider>();
            settingProvider.EmailActivation.Returns(true);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var email = EmailHelper.Generate();

            var regDto = userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", mailer);

            userService.Activate(Identity.Next(), Identity.Next());
        }

        [TestMethod]
        [ExpectedException(typeof(ActivationException))]
        public void ActivationNotNeededTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();
            authenticationContext.GetContextUser().Returns((string)null);
            var userOperations = environmentFactory.MgmtUserOperations;

            var settingProvider = Substitute.For<ISettingProvider>();
            settingProvider.EmailActivation.Returns(false);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var email = EmailHelper.Generate();

            var userId = userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", null);

            userService.Activate(userId, Identity.Next());
        }

        [TestMethod]
        [ExpectedException(typeof(ActivationException))]
        public void ActivationInvalidActivationCodeTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var mailer = Substitute.For<IMailer>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();
            authenticationContext.GetContextUser().Returns((string)null);
            var userOperations = environmentFactory.MgmtUserOperations;
            var settingProvider = Substitute.For<ISettingProvider>();
            settingProvider.EmailActivation.Returns(true);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var email = EmailHelper.Generate();

            var userId = userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", mailer);

            userService.Activate(userId, Identity.Next());
        }

        #endregion

        #region ChangePassword

        [TestMethod]
        public void ChangePasswordTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var email = EmailHelper.Generate();

            var userId = userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", null);

            authenticationContext.GetContextUser().Returns(userId);
            
            userService.ChangePassword(new ChangePasswordDto {CurrentPassword = "password", NewPassword = "password2"});

            AssertionHelper.AssertThrows<AuthenticationException>(
                () =>
                    userService.ChangePassword(new ChangePasswordDto
                    {
                        CurrentPassword = "passwordinvalid",
                        NewPassword = "password2"
                    }));
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void ChangePasswordNotLoggedInTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            authenticationContext.GetContextUser().Returns((string) null);

            userService.ChangePassword(new ChangePasswordDto { CurrentPassword = "password", NewPassword = "password2" });
        }

        #endregion

        #region ResendActivationEmail

        [TestMethod]
        public void ResendActivationEmail()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var mailer = Substitute.For<IMailer>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();
            authenticationContext.GetContextUser().Returns((string)null);
            var userOperations = environmentFactory.MgmtUserOperations;

            var settingProvider = Substitute.For<ISettingProvider>();
            settingProvider.EmailActivation.Returns(true);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var email = EmailHelper.Generate();

            var userId = userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", mailer);
            
            userService.ResendActivationEmail(email, mailer);

            mailer.DidNotReceiveWithAnyArgs().SendForgotPasswordEmail(null, null, null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void ResendActivationEmailAuthenticated()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var email = EmailHelper.Generate();

            var userId = userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", null);

            userService.ResendActivationEmail(email, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ActivationException))]
        public void ResendActivationEmailAlreadyActivated()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var email = EmailHelper.Generate();

            var userId = userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", null);
            authenticationContext.GetContextUser().Returns((string)null);

            userService.ResendActivationEmail(email, null);
        }

        #endregion

        #region ForgotPassword flow

        [TestMethod]
        public void SendForgotPasswordEmail()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var mailer = Substitute.For<IMailer>();
         
            var email = EmailHelper.Generate();

            var userId = userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", null);

            authenticationContext.GetContextUser().Returns((string)null);

            userService.SendForgotPasswordEmail(email, mailer);

            mailer.Received().SendForgotPasswordEmail(userId, "new user", email, Arg.Any<string>(), Arg.Any<string>());
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void TrySendForgotPasswordEmailLoggedIn()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var email = EmailHelper.Generate();

            userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", null);

            userService.SendForgotPasswordEmail(email, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ActivationException))]
        public void TrySendForgotPasswordEmailNotActivated()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var mailer = Substitute.For<IMailer>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();
            authenticationContext.GetContextUser().Returns((string)null);
            var userOperations = environmentFactory.MgmtUserOperations;

            var settingProvider = Substitute.For<ISettingProvider>();
            settingProvider.EmailActivation.Returns(true);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var email = EmailHelper.Generate();

            userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", mailer);

            authenticationContext.GetContextUser().Returns((string)null);

            userService.SendForgotPasswordEmail(email, null);
        }

        [TestMethod]
        public void ResetPassword()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var mailer = Substitute.For<IMailer>();

            var email = EmailHelper.Generate();

            var userId = userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", null);

            authenticationContext.GetContextUser().Returns((string)null);

            string confirmationCode = null;
            mailer.When(
                m => m.SendForgotPasswordEmail(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()))
                .Do(call => { confirmationCode = (string)call.Args()[3]; });

            userService.SendForgotPasswordEmail(email, mailer);

            userService.ResetPassword(new ResetPasswordDto
            {
                ConfirmationCode = confirmationCode,
                Password = "password2",
                UserId = userId
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ConfirmationException))]
        public void TryResetPasswordBadConfirmationCode()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var mailer = Substitute.For<IMailer>();

            var email = EmailHelper.Generate();

            var userId = userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", null);

            authenticationContext.GetContextUser().Returns((string)null);

            userService.SendForgotPasswordEmail(email, mailer);

            userService.ResetPassword(new ResetPasswordDto
            {
                ConfirmationCode = "fck",
                Password = "password2",
                UserId = userId
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ActivationException))]
        public void TryResetPasswordEmailNotActivated()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var mailer = Substitute.For<IMailer>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();
            authenticationContext.GetContextUser().Returns((string)null);
            var userOperations = environmentFactory.MgmtUserOperations;

            var settingProvider = Substitute.For<ISettingProvider>();
            settingProvider.EmailActivation.Returns(true);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var email = EmailHelper.Generate();

            var userId = userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", mailer);

            authenticationContext.GetContextUser().Returns((string)null);

            userService.ResetPassword(new ResetPasswordDto
            {
                ConfirmationCode = "fck",
                Password = "password2",
                UserId = userId
            });
        }

        #endregion

        #region ListCompanies
        [TestMethod]
        public void ListCompaniesTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var companyOperations = environmentFactory.MgmtCompanyOperations;
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);
            var userId = userService.Register(new RegisterDto() { Name = "user", Email = EmailHelper.Generate() }, "password", null);

            var companyService = new CompanyService(companyOperations, authenticationContext, null, new CapabilityProvider(settingProvider));

            authenticationContext.GetContextUser().Returns(userId);

            var compId = companyService.Create("new company");

            var companies = userService.ListCompanies();

            Assert.AreEqual(1, companies.Count);
            Assert.AreEqual(compId, companies[0].Id);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void ListCompaniesNotAuthenticatedTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.MgmtUserOperations;

            var userService = new UserService(userOperations, authenticationContext, null, null);

            authenticationContext.GetContextUser().Returns((string)null);

            userService.ListCompanies();
        }

        #endregion

        #region GetMe
        [TestMethod]
        public void GetMeTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var email = EmailHelper.Generate();

            var userId = userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", null);

            authenticationContext.GetContextUser().Returns(userId);

            var user = userService.GetMe();

            Assert.AreEqual(userId, user.Id);
            Assert.AreEqual("new user", user.Name);
            Assert.AreEqual(email, user.Email);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void GetMeNotAuthTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.MgmtUserOperations;

            var userService = new UserService(userOperations, authenticationContext, null, null);

            authenticationContext.GetContextUser().Returns((string)null);

            userService.GetMe();
        }

        #endregion

        #region FindUser

		[TestMethod]
        public void FindUserTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var email = EmailHelper.Generate();

            var userId = userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", null);

            var foundUser = userService.FindUser(email);
            Assert.AreEqual(userId, foundUser.Id);
            Assert.AreEqual("new user", foundUser.Name);
        }

        [TestMethod]
        public void FindUserNoSuchTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var email = EmailHelper.Generate();

            userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", null);

            var user = userService.FindUser(email + "asdfasfd");
            Assert.IsNull(user);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void FindUserNotAuthenticatedTest()
        {
            var authenticationContext = Substitute.For<IAuthenticationContext>();
            authenticationContext.GetContextUser().Returns((string) null);

            var userService = new UserService(null, authenticationContext, null, null);
            userService.FindUser("asdfasfd");
        }

	    #endregion    
    }
}
