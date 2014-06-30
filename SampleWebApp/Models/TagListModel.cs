using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SampleWebApp.Models
{
    public class TagListModel
    {
        [HiddenInput(DisplayValue = false)]
        public int TagId { get; set; }

        public string Slug { get; set; }

        public string Name { get; set; }

        public int NumPosts { get; set; }


    }
}