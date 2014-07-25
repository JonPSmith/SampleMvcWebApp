using System.Linq;
using System.Web.Mvc;
using DataLayer.DataClasses.Concrete;
using GenericServices;

namespace SampleWebApp.Models
{
    public class TagListModel
    {
        [HiddenInput(DisplayValue = false)]
        public int TagId { get; set; }

        public string Slug { get; set; }

        public string Name { get; set; }

        public int NumPosts { get; set; }

        public static IQueryable<TagListModel> GetListModels(IListService service)
        {
            return service.GetList<Tag>().Select(x => new TagListModel
            {
                TagId = x.TagId,
                Name = x.Name,
                Slug = x.Slug,
                NumPosts = x.Posts.Count()
            });
        }
    }
}