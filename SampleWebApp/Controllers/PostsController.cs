using System.Linq;
using System.Threading;
using System.Web.Mvc;
using DataLayer.DataClasses;
using DataLayer.DataClasses.Concrete;
using DataLayer.Startup;
using GenericServices;
using SampleWebApp.Infrastructure;
using ServiceLayer.PostServices.Concrete;


namespace SampleWebApp.Controllers
{
    /// <summary>
    /// This is an example of a Controller using GenericServices database commands with a DTO.
    /// In this case we are using normal, non-async commands
    /// </summary>
    public class PostsController : Controller
    {
        /// <summary>
        /// Note that is Index is different in that it has an optional id to filter the list on.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public ActionResult Index(int? id, IListService service)
        {
            var filtered = id != null && id != 0;
            var query = filtered ? service.GetList<SimplePostDto>().Where(x => x.BlogId == id) : service.GetList<SimplePostDto>();
            if (filtered)
                TempData["message"] = "Filtered list";

            return View(query.ToList());
        }

        public ActionResult Details(int id, IDetailService service)
        {
            return View(service.GetDetail<DetailPostDto>(id));
        }

        public ActionResult Edit(int id, IUpdateSetupService service)
        {
            var dto = service.GetOriginal<DetailPostDto>(id);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DetailPostDto dto, IUpdateService service)
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

        public ActionResult Create(ICreateSetupService setupService)
        {
            var dto = setupService.GetDto<DetailPostDto>();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DetailPostDto dto, ICreateService service)
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

        public ActionResult Delete(int id, IDeleteService service)
        {

            var response = service.Delete<Post>(id);
            if (response.IsValid)
                TempData["message"] = response.SuccessMessage;
            else
            {
                //else errors, so set up as error message
                TempData["errorMessage"] = new MvcHtmlString(response.ErrorsAsHtml());
            }
            return RedirectToAction("Index");
        }

        //--------------------------------------------

        public ActionResult CodeView()
        {
            return View();
        }

        public ActionResult Delay()
        {
            Thread.Sleep(500);
            return View(500);
        }

        public ActionResult Reset(SampleWebAppDb db)
        {
            DataLayerInitialise.ResetDatabaseToTestData(db, TestDataSelection.Medium);
            TempData["message"] = "Successfully reset the database";
            return RedirectToAction("Index");
        }
    }
}