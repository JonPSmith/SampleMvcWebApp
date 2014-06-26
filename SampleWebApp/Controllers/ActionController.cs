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
        public JsonResult Indeterminate(int id, string foo)
        {
            var data = new CommsTestActionData();

            return RunnerSetupFactory<ICommsTestActionNoCancelEtc>.CreateRunnerAndReturnJsonResult(data);
        }


        [HttpPost]
        public JsonResult Immediate(int iterations)
        {
            var data = new CommsTestActionData
            {
                NumIterations = iterations
            };
            return RunnerSetupFactory<ICommsTestActionNormal>.CreateRunnerAndReturnJsonResult(data);
        }

        [HttpPost]
        public JsonResult SuccessExit(int iterations)
        {
            var data = new CommsTestActionData
            {
                NumIterations = iterations
            };
            return RunnerSetupFactory<ICommsTestActionExitOnSuccess>.CreateRunnerAndReturnJsonResult(data);
        }

        public ActionResult ActionNeedingData()
        {
            return View(new CommsTestActionData());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ActionNeedingData(CommsTestActionData data)
        {
            if (!ModelState.IsValid)
                //model errors so we return a errorDict to the ajax call
                return ModelState.ReturnModelErrorsAsJson();

            return RunnerSetupFactory<ICommsTestActionNormal>.CreateRunnerAndReturnJsonResult(data);
        }

    }
}