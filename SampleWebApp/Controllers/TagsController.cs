using System.Linq;
using System.Web.Mvc;
using DataLayer.DataClasses.Concrete;
using GenericServices;
using SampleWebApp.Infrastructure;
using SampleWebApp.Models;

namespace SampleWebApp.Controllers
{
    public class TagsController : Controller
    {
        /// <summary>
        /// This is an example of a Controller using GenericServices database commands directly to the data class.
        /// In this case we are using normal, non-async commands
        /// </summary>
        public ActionResult Index(IListService<Tag> service)
        {
            return View(service.GetList().Select( x => new TagListModel
            {
                TagId = x.TagId,
                Name = x.Name,
                Slug = x.Slug,
                NumPosts = x.Posts.Count()
            }).ToList());
        }

        public ActionResult Details(int id, IDetailService<Tag> service)
        {
            return View(service.GetDetail(x => x.TagId == id));
        }


        public ActionResult Edit(int id, IDetailService<Tag> service)
        {
            return View(service.GetDetail(x => x.TagId == id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tag tag, IUpdateService<Tag> service)
        {
            if (!ModelState.IsValid)
                //model errors so return immediately
                return View(tag);

            var response = service.Update(tag);
            if (response.IsValid)
            {
                TempData["message"] = response.SuccessMessage;
                return RedirectToAction("Index");
            }

            //else errors, so copy the errors over to the ModelState and return to view
            response.CopyErrorsToModelState(ModelState, tag);
            return View(tag);
        }

        public ActionResult Create()
        {
            return View(new Tag());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tag tag, ICreateService<Tag> service)
        {
            if (!ModelState.IsValid)
                //model errors so return immediately
                return View(tag);

            var response = service.Create(tag);
            if (response.IsValid)
            {
                TempData["message"] = response.SuccessMessage;
                return RedirectToAction("Index");
            }

            //else errors, so copy the errors over to the ModelState and return to view
            response.CopyErrorsToModelState(ModelState, tag);
            return View(tag);
        }

        public ActionResult Delete(int id, IDeleteService<Tag> service)
        {

            var response = service.Delete(id);
            if (response.IsValid)
                TempData["message"] = response.SuccessMessage;
            else
            {
                //else errors, so set up as error message
                TempData["errorMessage"] = new MvcHtmlString(response.ErrorsAsHtml());
            }
            return RedirectToAction("Index");
        }

    }
}