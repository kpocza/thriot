using Microsoft.AspNet.Mvc;

namespace Thriot.Web.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult ForgotPasswordSend()
        {
            return View();
        }

        public IActionResult ActivationResend()
        {
            return View();
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }
    }
}