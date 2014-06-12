using System.Web.Mvc;
using DataLayer.DataClasses;
using DataLayer.DataClasses.Concrete;
using DataLayer.Startup;
using GenericServices;
using Microsoft.Owin.Security.Provider;
using SampleWebApp.ActionProgress;
using SampleWebApp.Infrastructure;
using ServiceLayer.PostServices.Concrete;
using ServiceLayer.TestActionService;
using ServiceLayer.TestActionService.Concrete;

namespace SampleWebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(IListService<Post, SimplePostDto> service)
        {
            return View(service.GetList());
        }

        public ActionResult Details(int id, IDetailService<Post, DetailPostDto> service)
        {
            return View(service.GetDetail(x => x.PostId == id));
        }


        public ActionResult Edit(int id, IUpdateSetupService<Post, DetailPostDto> service)
        {
            var dto = service.GetOriginal(x => x.PostId == id);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DetailPostDto dto, IUpdateService<Post, DetailPostDto> service)
        {
            if (!ModelState.IsValid)
                //model errors so return immediately
                return View(service.ResetDto(dto));

            var response = service.Update(dto);
            if (response.IsValid)
            {
                TempData["message"] = response.SuccessMessage;
                return RedirectToAction("Index");
            }

            //else errors, so copy the errors over to the ModelState and return to view
            response.CopyErrorsToModelState(ModelState, dto);
            return View(dto);
        }

        public ActionResult Create(ICreateSetupService<Post,DetailPostDto> setupService)
        {
            var dto = setupService.GetDto();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DetailPostDto dto, ICreateService<Post, DetailPostDto> service)
        {
            if (!ModelState.IsValid)
                //model errors so return immediately
                return View(service.ResetDto(dto));

            var response = service.Create(dto);
            if (response.IsValid)
            {
                TempData["message"] = response.SuccessMessage;
                return RedirectToAction("Index");
            }

            //else errors, so copy the errors over to the ModelState and return to view
            response.CopyErrorsToModelState(ModelState, dto);
            return View(dto);
        }

        public ActionResult Delete(int id, IDeleteService<Post> service)
        {

            var response = service.Delete(id);
            if (response.IsValid)
                TempData["message"] = response.SuccessMessage;
            else
                //else errors, so copy the errors over to the ModelState and return to view
                response.CopyErrorsToModelState(ModelState);
            return RedirectToAction("Index");
        }

        public ActionResult Reset(SampleWebAppDb db)
        {
            DataLayerInitialise.ResetDatabaseToTestData(db, TestDataSelection.Medium);
            TempData["message"] = "Successfully reset the database";
            return RedirectToAction("Index");
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