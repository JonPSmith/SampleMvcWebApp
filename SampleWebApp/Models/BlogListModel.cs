using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SampleWebApp.Models
{
    public class BlogListModel
    {
        [HiddenInput(DisplayValue = false)]
        public int BlogId { get; set; }

        public string Name { get; set; }

        public string EmailAddress { get; set; }

        public int NumPosts { get; set; }

    }
}