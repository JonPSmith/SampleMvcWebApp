using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SampleWebApp.ActionProgress;
using SampleWebApp.Infrastructure;
using ServiceLayer.TestActionService;
using ServiceLayer.TestActionService.Concrete;

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


        [HttpPost]
        public ActionResult Immediate(int iterations)
        {
            var data = new CommsTestActionData
            {
                NumIterations = iterations
            };
            return RunnerSetupFactory<ICommsTestAction>.CreateRunnerAndReturnJsonNetResult(data);
        }

        public ActionResult ActionNeedingData()
        {
            return View(new CommsTestActionData());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActionNeedingData(CommsTestActionData data)
        {
            if (!ModelState.IsValid)
                //model errors so we return a errorDict to the ajax call
                return ModelState.ReturnModelErrorsAsJson();

            return RunnerSetupFactory<ICommsTestAction>.CreateRunnerAndReturnJsonNetResult(data);
        }

    }
}