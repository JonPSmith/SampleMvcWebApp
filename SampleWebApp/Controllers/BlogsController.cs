using System.Linq;
using System.Web.Mvc;
using BizLayer.BlogsAnalysis;
using DataLayer.DataClasses.Concrete;
using GenericServices;
using SampleWebApp.Infrastructure;
using SampleWebApp.Models;

namespace SampleWebApp.Controllers
{

    /// <summary>
    /// This is an example of a Controller using GenericServices database commands directly to the data class.
    /// In this case we are using normal, non-async commands
    /// </summary>
    public class BlogsController : Controller
    {
       
        public ActionResult Index(IListService<Blog> service)
        {
            return View(service.GetList().Select(x => new BlogListModel
            {
                BlogId = x.BlogId,
                Name = x.Name,
                EmailAddress = x.EmailAddress,
                NumPosts = x.Posts.Count()
            }).ToList());
        }

        public ActionResult Analyse(int id, IBlogAnalyser service)
        {
            var response = service.DoAction(id);
            if (response.IsValid)
            {
                TempData["message"] = response.SuccessMessage;
                return View(response.Result);
            }

            //else errors, so set up as error message
            TempData["errorMessage"] = new MvcHtmlString(response.ErrorsAsHtml());
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id, IDetailService<Blog> service)
        {
            return View(service.GetDetail(x => x.BlogId == id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Blog blog, IUpdateService<Blog> service)
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
        public ActionResult Create(Blog blog, ICreateService<Blog> service)
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

        public ActionResult Delete(int id, IDeleteService<Blog> service)
        {

            var response = service.Delete(id);
            if (response.IsValid)
                TempData["message"] = response.SuccessMessage;
            else
            {
                //else errors, so set up as error message
                TempData["errorMessage"] = new MvcHtmlString(response.ErrorsAsHtml());
            }
            return RedirectToAction("Index");
        }


    }
}