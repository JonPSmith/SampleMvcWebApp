#region licence
// The MIT License (MIT)
// 
// Filename: PostsAsyncController.cs
// Date Created: 2014/06/17
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
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DataLayer.DataClasses;
using DataLayer.DataClasses.Concrete;
using DataLayer.Startup;
using GenericServices;
using SampleWebApp.Infrastructure;
using ServiceLayer.PostServices;

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
            return View(await service.GetAll<SimplePostDtoAsync>().ToListAsync());
        }

        public async Task<ActionResult> Details(int id, IDetailServiceAsync service)
        {
            return View((await service.GetDetailAsync<DetailPostDtoAsync>(id)).Result);
        }


        public async Task<ActionResult> Edit(int id, IUpdateSetupServiceAsync service)
        {
            return View((await service.GetOriginalAsync<DetailPostDtoAsync>(id)).Result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(DetailPostDtoAsync dto, IUpdateServiceAsync service)
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

        public async Task<ActionResult> Create(ICreateSetupServiceAsync setupService)
        {
            var dto = await setupService.GetDtoAsync<DetailPostDtoAsync>();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DetailPostDtoAsync dto, ICreateServiceAsync service)
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

        public async Task<ActionResult> Delete(int id, IDeleteServiceAsync service)
        {

            var response = await service.DeleteAsync<Post>(id);
            if (response.IsValid)
                TempData["message"] = response.SuccessMessage;
            else
                //else errors, so send back an error message
                TempData["errorMessage"] = new MvcHtmlString(response.ErrorsAsHtml());
           
            return RedirectToAction("Index");
        }

        //-----------------------------------------------------
        //Code used in https://www.simple-talk.com/dotnet/.net-framework/the-.net-4.5-asyncawait-commands-in-promise-and-practice/

        public async Task<ActionResult> NumPosts(SampleWebAppDb db)
        {
            return View((object)await GetNumPostsAsync(db));
        }

        private static async Task<string> GetNumPostsAsync(SampleWebAppDb db)
        {
            var numPosts = await db.Posts.CountAsync();
            return string.Format("The total number of Posts is {0}", numPosts);
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
            DataLayerInitialise.ResetBlogs(db, TestDataSelection.Medium);
            TempData["message"] = "Successfully reset the blogs data";
            return RedirectToAction("Index");
        }
    }
}