using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataLayer.DataClasses;
using DataLayer.DataClasses.Concrete;
using DataLayer.Startup;
using GenericServices;
using SampleWebApp.Infrastructure;
using SampleWebApp.Models;
using ServiceLayer.PostServices.Concrete;

namespace SampleWebApp.Controllers
{
    public class TagsAsyncController : Controller
    {
        // GET: TagsAsync
        public async Task<ActionResult> Index(IListService<Tag> service)
        {
            return View(await service.GetList().Select( x => new TagListModel
            {
                TagId = x.TagId,
                Name = x.Name,
                Slug = x.Slug,
                NumPosts = x.Posts.Count()
            }).ToListAsync());
        }

        public async Task<ActionResult> Details(int id, IDetailServiceAsync<Tag> service)
        {
            return View(await service.GetDetailAsync(x => x.TagId == id));
        }


        public async Task<ActionResult> Edit(int id, IDetailServiceAsync<Tag> service)
        {
            return View(await service.GetDetailAsync(x => x.TagId == id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Tag tag, IUpdateServiceAsync<Tag> service)
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
        public async Task<ActionResult> Create(Tag tag, ICreateServiceAsync<Tag> service)
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

        public async Task<ActionResult> Delete(int id, IDeleteServiceAsync<Tag> service)
        {

            var response = await service.DeleteAsync(id);
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