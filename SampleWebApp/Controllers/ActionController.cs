using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SampleWebApp.Controllers
{
    public class ActionController : Controller
    {
        // GET: Action
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Indeterminate(int id, string foo)
        {
            System.Threading.Thread.Sleep(5000);

            return Json("Message from Post action method.");
        }
    }
}