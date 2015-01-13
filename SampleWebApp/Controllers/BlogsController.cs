#region licence
// The MIT License (MIT)
// 
// Filename: BlogsController.cs
// Date Created: 2014/07/11
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
using System.Linq;
using System.Web.Mvc;
using DataLayer.DataClasses.Concrete;
using GenericServices;
using SampleWebApp.Infrastructure;
using ServiceLayer.BlogServices;

namespace SampleWebApp.Controllers
{

    /// <summary>
    /// This is an example of a Controller using GenericServices database commands directly to the data class (other that List, which needs a DTO)
    /// In this case we are using normal, non-async commands
    /// </summary>
    public class BlogsController : Controller
    {
       
        public ActionResult Index(IListService service)
        {
            return View(service.GetAll<BlogListDto>().ToList());
        }

        public ActionResult Edit(int id, IUpdateSetupService service)
        {
            return View(service.GetOriginal<Blog>(id).Result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Blog blog, IUpdateService service)
        {
            if (!ModelState.IsValid)
                //model errors so return immediately
                return View(blog);

            var response = service.Update(blog);
            if (response.IsValid)
            {
                TempData["message"] = response.SuccessMessage;
                return RedirectToAction("Index");
            }

            //else errors, so copy the errors over to the ModelState and return to view
            response.CopyErrorsToModelState(ModelState, blog);
            return View(blog);
        }

        public ActionResult Create()
        {
            return View(new Blog());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Blog blog, ICreateService service)
        {
            if (!ModelState.IsValid)
                //model errors so return immediately
                return View(blog);

            var response = service.Create(blog);
            if (response.IsValid)
            {
                TempData["message"] = response.SuccessMessage;
                return RedirectToAction("Index");
            }

            //else errors, so copy the errors over to the ModelState and return to view
            response.CopyErrorsToModelState(ModelState, blog);
            return View(blog);
        }

        public ActionResult Delete(int id, IDeleteService service)
        {

            var response = service.Delete<Blog>(id);
            if (response.IsValid)
                TempData["message"] = response.SuccessMessage;
            //else it throws a concurrecy error, which shows the default error page.

            return RedirectToAction("Index");
        }


    }
}