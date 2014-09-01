using System.Net;
using System.Web.Mvc;
using SampleWebApp.Identity;
using SampleWebApp.Infrastructure;
using SampleWebApp.Properties;
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
            return View(service.GetSqlCommands(Settings.Default.DatabaseLoginPrefix));
        }

        public ActionResult ExecuteSqlCommands(ISqlCommands service)
        {
            var status = service.ExecuteSqlCommandsFromFile(
                System.Web.HttpContext.Current.Server.MapPath("~/App_Data/"),
                WebUiInitialise.HostType.ToString());

            if (status.IsValid)
                TempData["message"] = status.SuccessMessage;
            else
                //else errors, so send back an error message
                TempData["errorMessage"] = new MvcHtmlString(status.ErrorsAsHtml());

            return RedirectToAction("Index");
        }
    }
}