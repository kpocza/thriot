using System.Web.Mvc;
using IoT.Web.Models;

namespace IoT.Web.Controllers
{
    public class ReportController : Controller
    {
        [Route("Network/{id}")]
        public ActionResult Network(string id)
        {
            return View(new Mdl { Id = id });
        }

        public ActionResult Device()
        {
            return View();
        }
    }
}