using System.Web.Mvc;
using Thriot.Web.Models;

namespace Thriot.Web.Controllers
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