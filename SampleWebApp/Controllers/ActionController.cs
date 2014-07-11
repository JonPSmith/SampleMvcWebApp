using System.Web.Mvc;
using BizLayer.BBCScheduleService;
using BizLayer.BBCScheduleService.Concrete;
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
        public JsonResult Indeterminate(int id, string foo)
        {
            var data = new CommsTestActionData();

            return RunnerSetupFactory<ICommsTestActionNoCancelEtc>.SetupRunner(data);
        }


        [HttpPost]
        public JsonResult Immediate(int iterations)
        {
            var data = new CommsTestActionData
            {
                NumIterations = iterations
            };
            return RunnerSetupFactory<ICommsTestActionNormal>.SetupRunner(data);
        }

        [HttpPost]
        public JsonResult SuccessExit(int iterations)
        {
            var data = new CommsTestActionData
            {
                NumIterations = iterations
            };
            return RunnerSetupFactory<ICommsTestActionExitOnSuccess>.SetupRunner(data);
        }

        public ActionResult CommsWithData()
        {
            return View(new CommsTestActionData());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CommsWithData(CommsTestActionData data)
        {
            if (!ModelState.IsValid)
                //model errors so we return a errorDict to the ajax call
                return ModelState.ReturnModelErrorsAsJson();

            return RunnerSetupFactory<ICommsTestActionNormal>.SetupRunner(data);
        }

        public ActionResult Radio4Search()
        {
            return View(new ScheduleSearcherData());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Radio4Search(ScheduleSearcherData data)
        {
            if (!ModelState.IsValid)
                //model errors so we return a errorDict to the ajax call
                return ModelState.ReturnModelErrorsAsJson();

            return RunnerSetupFactory<IScheduleSearcherAsync>.SetupRunner(data);
        }

    }
}