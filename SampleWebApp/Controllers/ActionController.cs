#region licence
// The MIT License (MIT)
// 
// Filename: ActionController.cs
// Date Created: 2014/06/18
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
using System.Web.Mvc;
using BizLayer.BBCScheduleService;
using BizLayer.BBCScheduleService.Concrete;
using SampleWebApp.ActionProgress;
using SampleWebApp.Infrastructure;
using ServiceLayer.TestActionService;
using ServiceLayer.TestActionService.Concrete;

namespace SampleWebApp.Controllers
{
    public class ActionController : Controller
    {
        // GET: Action
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Indeterminate(int id, string foo)
        {
            var data = new CommsTestActionData();

            return RunnerSetupFactory<ICommsTestActionNoCancelEtc>.SetupRunner(data);
        }


        [HttpPost]
        public JsonResult Immediate(int iterations)
        {
            var data = new CommsTestActionData
            {
                NumIterations = iterations
            };
            return RunnerSetupFactory<ICommsTestActionNormal>.SetupRunner(data);
        }

        [HttpPost]
        public JsonResult SuccessExit(int iterations)
        {
            var data = new CommsTestActionData
            {
                NumIterations = iterations
            };
            return RunnerSetupFactory<ICommsTestActionExitOnSuccess>.SetupRunner(data);
        }

        public ActionResult CommsWithData()
        {
            return View(new CommsTestActionData());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CommsWithData(CommsTestActionData data)
        {
            if (!ModelState.IsValid)
                //model errors so we return a errorDict to the ajax call
                return ModelState.ReturnModelErrorsAsJson();

            return RunnerSetupFactory<ICommsTestActionNormal>.SetupRunner(data);
        }

        public ActionResult Radio4Search()
        {
            return View(new ScheduleSearcherData());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Radio4Search(ScheduleSearcherData data)
        {
            if (!ModelState.IsValid)
                //model errors so we return a errorDict to the ajax call
                return ModelState.ReturnModelErrorsAsJson();

            return RunnerSetupFactory<IScheduleSearcherAsync>.SetupRunner(data);
        }

    }
}