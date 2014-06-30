using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using DataLayer.DataClasses;
using DataLayer.DataClasses.Concrete;
using DataLayer.Startup;
using GenericServices;
using SampleWebApp.Infrastructure;
using ServiceLayer.PostServices.Concrete;

namespace SampleWebApp.Controllers
{
    public class PostsAsyncController : Controller
    {
        public async Task<ActionResult> Index(IListService<Post, SimplePostDto> service)
        {
            return View(await service.GetList().ToListAsync());
        }

        public async Task<ActionResult> Details(int id, IDetailServiceAsync<Post, DetailPostDtoAsync> service)
        {
            return View(await service.GetDetailAsync(x => x.PostId == id));
        }


        public async Task<ActionResult> Edit(int id, IUpdateSetupServiceAsync<Post, DetailPostDtoAsync> service)
        {
            var dto = await service.GetOriginalAsync(x => x.PostId == id);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(DetailPostDtoAsync dto, IUpdateServiceAsync<Post, DetailPostDtoAsync> service)
        {
            if (!ModelState.IsValid)
                //model errors so return immediately
                return View(await service.ResetDtoAsync(dto));

            var response = await service.UpdateAsync(dto);
            if (response.IsValid)
            {
                TempData["message"] = response.SuccessMessage;
                return RedirectToAction("Index");
            }

            //else errors, so copy the errors over to the ModelState and return to view
            response.CopyErrorsToModelState(ModelState, dto);
            return View(dto);
        }

        public async Task<ActionResult> Create(ICreateSetupServiceAsync<Post, DetailPostDtoAsync> setupService)
        {
            var dto = await setupService.GetDtoAsync();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DetailPostDtoAsync dto, ICreateServiceAsync<Post, DetailPostDtoAsync> service)
        {
            if (!ModelState.IsValid)
                //model errors so return immediately
                return View(await service.ResetDtoAsync(dto));

            var response = await service.CreateAsync(dto);
            if (response.IsValid)
            {
                TempData["message"] = response.SuccessMessage;
                return RedirectToAction("Index");
            }

            //else errors, so copy the errors over to the ModelState and return to view
            response.CopyErrorsToModelState(ModelState, dto);
            return View(dto);
        }

        public async Task<ActionResult> Delete(int id, IDeleteServiceAsync<Post> service)
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

        public async Task<ActionResult> Delay()
        {
            await Task.Delay(500);
            return RedirectToAction("Index");
        }

        public ActionResult Reset(SampleWebAppDb db)
        {
            DataLayerInitialise.ResetDatabaseToTestData(db, TestDataSelection.Medium);
            TempData["message"] = "Successfully reset the database";
            return RedirectToAction("Index");
        }
    }
}