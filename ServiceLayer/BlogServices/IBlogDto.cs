using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataLayer.DataClasses.Concrete;

namespace ServiceLayer.BlogServices
{
    public interface IBlogDto
    {
        [Key]
        int BlogId { get; set; }

        [MinLength(2)]
        [MaxLength(64)]
        [Required]
        string Name { get; set; }

        [MaxLength(256)]
        [Required]
        string EmailAddress { get; set; }

        List<Post> Posts { get; }

        /// <summary>
        /// AutoMapper will create a .Count() version of the TData property Posts
        /// </summary>
        int PostsCount { get; }
    }
}