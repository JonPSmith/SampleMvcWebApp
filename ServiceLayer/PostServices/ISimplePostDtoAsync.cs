using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataLayer.DataClasses.Concrete;

namespace ServiceLayer.PostServices
{
    public interface ISimplePostDtoAsync
    {
        [UIHint("HiddenInput")]
        [Key]
        int PostId { get; set; }

        string BloggerName { get; }

        [MinLength(2), MaxLength(128)]
        string Title { get; set; }

        ICollection<Tag> Tags { get; }
        DateTime LastUpdated { get; }

        /// <summary>
        /// When it was last updated in DateTime format
        /// </summary>
        DateTime LastUpdatedUtc { get; }

        string TagNames { get; }
        void CacheSetup();
    }
}