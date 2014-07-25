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
            return View((await TagListModel.GetListModels(service).ToListAsync()).ShowData());
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
        public async Task<ActionResult> Edit(Tag tag, IUpdateServiceAsync service, IListService listService)
        {
            if (!ModelState.IsValid)
                //model errors so return immediately
                return View(tag);

            var response = await service.UpdateAsync(tag);
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
        public async Task<ActionResult> Create(Tag tag, ICreateServiceAsync service, IListService listService)
        {
            if (!ModelState.IsValid)
                //model errors so return immediately
                return View(tag);

            var response = await service.CreateAsync(tag);
            if (response.IsValid)
                return View("Index", TagListModel.GetListModels(listService).ToList().ShowDataAndMessage(response));

            //else errors, so copy the errors over to the ModelState and return to view
            response.CopyErrorsToModelState(ModelState, tag);
            return View(tag);
        }

        public async Task<ActionResult> Delete(int id, IDeleteServiceAsync service, IListService listService)
        {
            var response = await service.DeleteAsync<Tag>(id);
            return View("Index", TagListModel.GetListModels(listService).ToList().ShowDataAndMessage(response));
        }

        //--------------------------------------------

        public ActionResult CodeView()
        {
            return View();
        }

    }
}