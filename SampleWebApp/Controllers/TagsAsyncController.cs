#region licence
// The MIT License (MIT)
// 
// Filename: TagsAsyncController.cs
// Date Created: 2014/06/30
// 
// Copyright (c) 2014 Jon Smith (www.selectiveanalytics.com & www.thereformedprogrammer.net)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using DataLayer.DataClasses.Concrete;
using GenericServices;
using SampleWebApp.Infrastructure;
using ServiceLayer.TagServices;

namespace SampleWebApp.Controllers
{
    /// <summary>
    /// This is an example of a Controller using GenericServices database commands directly to the data class (other that List, which needs a DTO)
    /// In this case we are using async commands
    /// </summary>
    public class TagsAsyncController : Controller
    {
        // GET: TagsAsync
        public async Task<ActionResult> Index(IListService service)
        {
            return View(await service.GetAll<TagListDto>().ToListAsync());
        }

        public async Task<ActionResult> Details(int id, IDetailServiceAsync service)
        {
            return View((await service.GetDetailAsync<Tag>(id)).Result);
        }


        public async Task<ActionResult> Edit(int id, IUpdateSetupServiceAsync service)
        {
            return View((await service.GetOriginalAsync<Tag>(id)).Result);
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