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
