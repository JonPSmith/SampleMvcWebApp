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