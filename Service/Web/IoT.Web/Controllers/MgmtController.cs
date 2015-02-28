using System.Web.Mvc;
using IoT.Web.Models;

namespace IoT.Web.Controllers
{
    public class MgmtController : Controller
    {
        [Route("Companies")]
        public ActionResult Companies()
        {
            return View();
        }

        [Route("Company/{id}")]
        public ActionResult Company(string id)
        {
            return View(new Mdl {Id = id});
        }

        [Route("Service/{id}")]
        public ActionResult Service(string id)
        {
            return View(new Mdl {Id = id});
        }

        [Route("Network/{id}")]
        public ActionResult Network(string id)
        {
            return View(new Mdl {Id = id});
        }

        [Route("Device/{id}")]
        public ActionResult Device(string id)
        {
            return View(new Mdl {Id = id});
        }
    }
}