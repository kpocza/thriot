using Microsoft.AspNet.Mvc;

namespace Thriot.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        public IActionResult Error()
        {
            ViewBag.Title = "Error";

            return View();
        }
    }
}
