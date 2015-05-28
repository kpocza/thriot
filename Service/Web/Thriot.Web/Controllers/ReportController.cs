using Microsoft.AspNet.Mvc;
using Thriot.Web.Models;

namespace Thriot.Web.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Network(string id)
        {
            return View(new Mdl { Id = id });
        }

        public IActionResult Device()
        {
            return View();
        }
    }
}