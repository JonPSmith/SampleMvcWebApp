using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
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
    public class PostsAsyncController : Controller
    {
        /// <summary>
        /// This is an example of a Controller using GenericServices database commands with a DTO.
        /// In this case we are using async commands
        /// </summary>
        public async Task<ActionResult> Index(IListService service)
        {
            return View((await service.GetList<SimplePostDtoAsync>().ToListAsync()).ShowData());
        }

        public async Task<ActionResult> Details(int id, IDetailServiceAsync service)
        {
            return View(await service.GetDetailAsync<DetailPostDtoAsync>(id));
        }


        public async Task<ActionResult> Edit(int id, IUpdateSetupServiceAsync service)
        {
            var dto = await service.GetOriginalAsync<DetailPostDtoAsync>(id);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(DetailPostDtoAsync dto, IUpdateServiceAsync service, IListService listService)
        {
            if (!ModelState.IsValid)
                //model errors so return immediately
                return View(await service.ResetDtoAsync(dto));

            var response = await service.UpdateAsync(dto);
            if (response.IsValid)
                return View("Index", listService.GetList<SimplePostDtoAsync>().ToList().ShowDataAndMessage(response));

            //else errors, so copy the errors over to the ModelState and return to view
            response.CopyErrorsToModelState(ModelState, dto);
            return View(dto);
        }

        public async Task<ActionResult> Create(ICreateSetupServiceAsync setupService)
        {
            var dto = await setupService.GetDtoAsync<DetailPostDtoAsync>();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DetailPostDtoAsync dto, ICreateServiceAsync service, IListService listService)
        {
            if (!ModelState.IsValid)
                //model errors so return immediately
                return View(await service.ResetDtoAsync(dto));

            var response = await service.CreateAsync(dto);
            if (response.IsValid)
                return View("Index", listService.GetList<SimplePostDtoAsync>().ToList().ShowDataAndMessage(response));

            //else errors, so copy the errors over to the ModelState and return to view
            response.CopyErrorsToModelState(ModelState, dto);
            return View(dto);
        }

        public async Task<ActionResult> Delete(int id, IDeleteServiceAsync service, IListService listService)
        {
            var response = await service.DeleteAsync<Post>(id);
            return View("Index", listService.GetList<SimplePostDtoAsync>().ToList().ShowDataAndMessage(response));
        }

        //--------------------------------------------

        public ActionResult CodeView()
        {
            return View();
        }

        public async Task<ActionResult> Delay()
        {
            await Task.Delay(500);
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