using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DataLayer.DataClasses.Concrete;
using GenericServices;
using SampleWebApp.Infrastructure;
using SampleWebApp.Models;

namespace SampleWebApp.Controllers
{
    /// <summary>
    /// This is an example of a Controller using GenericServices database commands directly to the data class.
    /// In this case we are using async commands
    /// </summary>
    public class TagsAsyncController : Controller
    {
        // GET: TagsAsync
        public async Task<ActionResult> Index(IListService service)
        {
            return View(await service.GetList<Tag>().Select(x => new TagListModel
            {
                TagId = x.TagId,
                Name = x.Name,
                Slug = x.Slug,
                NumPosts = x.Posts.Count()
            }).ToListAsync());
        }

        public async Task<ActionResult> Details(int id, IDetailServiceAsync service)
        {
            return View(await service.GetDetailAsync<Tag>(id));
        }


        public async Task<ActionResult> Edit(int id, IDetailServiceAsync service)
        {
            return View(await service.GetDetailAsync<Tag>(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Tag tag, IUpdateServiceAsync service)
        {
            if (!ModelState.IsValid)
                //model errors so return immediately
                return View(tag);

            var response = await service.UpdateAsync(tag);
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
        public async Task<ActionResult> Create(Tag tag, ICreateServiceAsync service)
        {
            if (!ModelState.IsValid)
                //model errors so return immediately
                return View(tag);

            var response = await service.CreateAsync(tag);
            if (response.IsValid)
            {
                TempData["message"] = response.SuccessMessage;
                return RedirectToAction("Index");
            }

            //else errors, so copy the errors over to the ModelState and return to view
            response.CopyErrorsToModelState(ModelState, tag);
            return View(tag);
        }

        public async Task<ActionResult> Delete(int id, IDeleteServiceAsync service)
        {

            var response = await service.DeleteAsync<Tag>(id);
            if (response.IsValid)
                TempData["message"] = response.SuccessMessage;
            else
                //else errors, so send back an error message
                TempData["errorMessage"] = new MvcHtmlString(response.ErrorsAsHtml());

            return RedirectToAction("Index");
        }

        //--------------------------------------------

        public ActionResult CodeView()
        {
            return View();
        }

    }
}