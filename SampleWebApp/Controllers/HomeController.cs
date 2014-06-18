using System.Web.Mvc;
using SampleWebApp.ActionProgress;
using SampleWebApp.Infrastructure;
using ServiceLayer.TestActionService;
using ServiceLayer.TestActionService.Concrete;

namespace SampleWebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        //-------------------------------------------------------

        public ActionResult RunAction()
        {
            return View(new CommsTestActionData());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RunAction(CommsTestActionData data)
        {
            if (!ModelState.IsValid)
                //model errors so we return a errorDict to the ajax call
                return ModelState.ReturnModelErrorsAsJson();

            return RunnerSetupFactory<ICommsTestAction>.CreateRunnerAndReturnJsonNetResult(data);
        }

        //-------------------------------------------------------


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}