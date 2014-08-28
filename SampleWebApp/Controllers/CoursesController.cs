using System.Web.Mvc;
using DataLayer.DataClasses;
using DataLayer.DataClasses.Concrete;
using DataLayer.Startup;
using GenericServices;
using GenericServices.Core;
using SampleWebApp.Infrastructure;
using ServiceLayer.CourseServices;

namespace SampleWebApp.Controllers
{
    public class CoursesController : Controller
    {
        public ActionResult Index(IListService service)
        {
            var status = service.GetMany<CourseListDto>().TryManyWithPermissionChecking();

            if (!status.IsValid)
                TempData["errorMessage"] = new MvcHtmlString(status.ErrorsAsHtml());
                  
            return View(status.Result);
        }


        public ActionResult Details(int id, IDetailService service)
        {
            var status = service.GetDetail<CourseDetailDto>(id);
            if (status.IsValid)
                return View(status.Result);

            TempData["errorMessage"] = new MvcHtmlString(status.ErrorsAsHtml());
            return RedirectToAction("Index");
        }


        public ActionResult Edit(int id, IDetailService service)
        {
            var status = service.GetDetail<CourseDetailDto>(id);
            if (status.IsValid)
                return View(status.Result);

            TempData["errorMessage"] = new MvcHtmlString(status.ErrorsAsHtml());
            return RedirectToAction("Index");
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
            else
                //else errors, so send back an error message
                TempData["errorMessage"] = new MvcHtmlString(response.ErrorsAsHtml());

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