using System.Linq;
using System.Web.Mvc;
using DataLayer.DataClasses.Concrete;
using GenericServices;

namespace SampleWebApp.Models
{
    public class BlogListModel
    {
        [HiddenInput(DisplayValue = false)]
        public int BlogId { get; set; }

        public string Name { get; set; }

        public string EmailAddress { get; set; }

        public int NumPosts { get; set; }

        public static IQueryable<BlogListModel> GetListModels(IListService service)
        {
            return service.GetList<Blog>().Select(x => new BlogListModel
            {
                BlogId = x.BlogId,
                Name = x.Name,
                EmailAddress = x.EmailAddress,
                NumPosts = x.Posts.Count()
            });
        }
    }
}