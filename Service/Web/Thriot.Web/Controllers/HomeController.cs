using System.Web.Mvc;

namespace Thriot.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        public ActionResult Error()
        {
            ViewBag.Title = "Error";

            return View();
        }
    }
}
