using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataLayer.DataClasses;
using DataLayer.Security;
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

        //-------------------------------------------------
        //sql commands

        public ActionResult ViewPermissions(ISqlCommands service)
        {
            return View(service.GetSqlCommands());
        }
    }
}