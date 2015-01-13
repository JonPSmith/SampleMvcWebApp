#region licence
// The MIT License (MIT)
// 
// Filename: TagsController.cs
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
using System.Linq;
using System.Web.Mvc;
using DataLayer.DataClasses.Concrete;
using GenericServices;
using SampleWebApp.Infrastructure;
using ServiceLayer.TagServices;

namespace SampleWebApp.Controllers
{
    public class TagsController : Controller
    {
        /// <summary>
        /// This is an example of a Controller using GenericServices database commands directly to the data class (other that List, which needs a DTO)
        /// In this case we are using normal, non-async commands
        /// </summary>
        public ActionResult Index(IListService service)
        {
            return View(service.GetAll<TagListDto>().ToList());
        }

        public ActionResult Details(int id, IDetailService service)
        {
            return View(service.GetDetail<Tag>(id).Result);
        }


        public ActionResult Edit(int id, IUpdateSetupService service)
        {
            return View(service.GetOriginal<Tag>(id).Result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tag tag, IUpdateService service)
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
        public ActionResult Create(Tag tag, ICreateService service)
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

        public ActionResult Delete(int id, IDeleteService service)
        {

            var response = service.Delete<Tag>(id);
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