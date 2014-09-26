#region licence
// The MIT License (MIT)
// 
// Filename: ModelStateTester.cs
// Date Created: 2014/06/10
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
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;

namespace Tests.Helpers
{
    static class ModelStateTester
    {

        public class TestModel : IValidatableObject
        {
            [MinLength(2)]
            [Required]
            public string MyString { get; set; }

            [Range(0,100)]
            public int MyInt { get; set; }

            public bool CreateValidationError { get; set; }

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                if (CreateValidationError)
                    yield return new ValidationResult("This is a top level error caused by CreateValidationError being set.");

                if (MyInt == 50)
                    yield return new ValidationResult("This is a top level error caused by MyInt having value 50.");
            }

            public TestModel()
            {
            }

            public TestModel(string myString, int myInt, bool createValidationError)
            {
                MyString = myString;
                MyInt = myInt;
                CreateValidationError = createValidationError;
            }
        }


        private class TestController : Controller
        {
            public ActionResult ValidDateTestModel(TestModel model)
            {
                // ReSharper disable once Mvc.ViewNotResolved
                return View(model);
            }
        }

        public static ModelStateDictionary ReturnModelState(this TestModel model)
        {
            var testController = new TestController();

            var modelBinder = new ModelBindingContext()
            {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(
                                  () => model, model.GetType()),
                ValueProvider = new NameValueCollectionValueProvider(
                                    new NameValueCollection(), CultureInfo.InvariantCulture)
            };
            var binder = new DefaultModelBinder().BindModel(
                             new ControllerContext(), modelBinder);
            testController.ModelState.Clear();
            testController.ModelState.Merge(modelBinder.ModelState);

            var viewResult = (ViewResult) testController.ValidDateTestModel(model);
            return viewResult.ViewData.ModelState;
        }


    }
}
