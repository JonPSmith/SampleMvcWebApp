using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using DataLayer.DataClasses.Concrete;
using GenericServices;
using GenericServices.Core;
using GenericServices.Services;

namespace ServiceLayer.BlogServices.Concrete
{
    public class BlogDto : EfGenericDto<Blog,BlogDto>, IBlogDto
    {
        [Key]
        public int BlogId { get; set; }
        [MinLength(2)]
        [MaxLength(64)]
        [Required]
        public string Name { get; set; }

        [MaxLength(256)]
        [Required]
        public string EmailAddress { get; set; }

        public List<Post> Posts { get; protected set; }         //you can't update the list of posts this blogger has

        /// <summary>
        /// AutoMapper will create a .Count() version of the TData property Posts
        /// </summary>
        public int PostsCount { get; protected set; }


        protected override ServiceFunctions SupportedFunctions
        {
            get { return ServiceFunctions.AllCrudButCreate | ServiceFunctions.DoesNotNeedSetup; }
        }

        /// <summary>
        /// We ensure that the Posts data is included
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override IQueryable<Blog> GetDataUntracked(IDbContextWithValidation context)
        {
            return base.GetDataUntracked(context).Include(x => x.Posts);
        }
    }
}
