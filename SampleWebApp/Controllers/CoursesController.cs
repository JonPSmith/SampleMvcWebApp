using System.Linq;
using System.Web.Mvc;
using DataLayer.DataClasses;
using DataLayer.DataClasses.Concrete;
using DataLayer.Startup;
using GenericServices;
using SampleWebApp.Infrastructure;
using ServiceLayer.CourseServices;

namespace SampleWebApp.Controllers
{
    public class CoursesController : Controller
    {
        public ActionResult Index(IListService service)
        {
            return View(service.GetList<CourseListDto>().ToList());
        }


        public ActionResult Details(int id, IDetailService service)
        {
            return View(service.GetDetail<CourseDetailDto>(id));
        }


        public ActionResult Edit(int id, IDetailService service)
        {
            return View(service.GetDetail<CourseDetailDto>(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CourseDetailDto dto, IUpdateService service)
        {
            if (!ModelState.IsValid)
                //model errors so return immediately
                return View(dto);

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

        public ActionResult Create(ICreateSetupService service)
        {
            return View(service.GetDto<CourseDetailDto>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CourseDetailDto dto, ICreateService service)
        {
            if (!ModelState.IsValid)
                //model errors so return immediately
                return View(dto);

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

        public ActionResult Delete(int id, IDeleteService service)
        {

            var response = service.Delete<Course>(id);
            if (response.IsValid)
                TempData["message"] = response.SuccessMessage;
            //else it throws a concurrecy error, which shows the default error page.

            return RedirectToAction("Index");
        }


        //--------------------------------------------

        public ActionResult Reset(SampleWebAppDb db)
        {
            DataLayerInitialise.ResetCourses(db);
            TempData["message"] = "Successfully reset the courses data";
            return RedirectToAction("Index");
        }

    }
}