using Microsoft.AspNet.Mvc;
using Thriot.Web.Models;

namespace Thriot.Web.Controllers
{
    public class MgmtController : Controller
    {
        public IActionResult Companies()
        {
            return View();
        }

        public IActionResult Company(string id)
        {
            return View(new Mdl {Id = id});
        }

        public IActionResult Service(string id)
        {
            return View(new Mdl {Id = id});
        }

        public IActionResult Network(string id)
        {
            return View(new Mdl {Id = id});
        }

        public IActionResult Device(string id)
        {
            return View(new Mdl {Id = id});
        }
    }
}