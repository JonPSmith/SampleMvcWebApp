using System.Net;
using System.Web.Mvc;
using SampleWebApp.Identity;
using ServiceLayer.Security;

namespace SampleWebApp.Controllers
{
    public class SecurityController : Controller
    {
        // GET: Security
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult PartialUsers()
        {
            return PartialView(new SelectListOfUsers(HttpContext));
        }

        [HttpPost]
        public ActionResult SetUser(string value)
        {
            HttpContext.ChangeUser(value);
            return new HttpStatusCodeResult(HttpStatusCode.Accepted);
        }

        public ActionResult CodeView()
        {
            return View();
        }

        //-------------------------------------------------
        //sql commands

        public ActionResult ViewPermissions(ISqlCommands service)
        {
            return View(service.GetSqlCommands());
        }
    }
}