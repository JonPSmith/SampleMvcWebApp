using System.Collections.Generic;
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
        public ActionResult Index(IListService service)
        {
            return View(TagListModel.GetListModels(service).ToList().ShowData());
        }

        public ActionResult Details(int id, IDetailService service)
        {
            return View(service.GetDetail<Tag>(id));
        }


        public ActionResult Edit(int id, IDetailService service)
        {
            return View(service.GetDetail<Tag>(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tag tag, IUpdateService service, IListService listService)
        {
            if (!ModelState.IsValid)
                //model errors so return immediately
                return View(tag);

            var response = service.Update(tag);
            if (response.IsValid)
                return View("Index", TagListModel.GetListModels(listService).ToList().ShowDataAndMessage(response));

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
        public ActionResult Create(Tag tag, ICreateService service, IListService listService)
        {
            if (!ModelState.IsValid)
                //model errors so return immediately
                return View(tag);

            var response = service.Create(tag);
            if (response.IsValid)
                return View("Index", TagListModel.GetListModels(listService).ToList().ShowDataAndMessage(response));

            //else errors, so copy the errors over to the ModelState and return to view
            response.CopyErrorsToModelState(ModelState, tag);
            return View(tag);
        }

        public ActionResult Delete(int id, IDeleteService service, IListService listService)
        {
            var response = service.Delete<Tag>(id);
            return View("Index", TagListModel.GetListModels(listService).ToList().ShowDataAndMessage(response));
        }

        //--------------------------------------------

        public ActionResult CodeView()
        {
            return View();
        }


    }
}