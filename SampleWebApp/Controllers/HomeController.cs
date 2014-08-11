using System.Web.Mvc;
using SampleWebApp.Models;

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
            return View();
        }

        public ActionResult Internals()
        {
            return View(new InternalsInfo());
        }

        public ActionResult CodeView()
        {
            return View();
        }
    }
}