#region licence
// The MIT License (MIT)
// 
// Filename: DetailPostDtoAsync.cs
// Date Created: 2014/08/16
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
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DataLayer.DataClasses;
using DataLayer.DataClasses.Concrete;
using GenericLibsBase;
using GenericLibsBase.Core;
using GenericServices;
using GenericServices.Core;
using ServiceLayer.UiClasses;

[assembly: InternalsVisibleTo("Tests")]

namespace ServiceLayer.PostServices
{
    public class DetailPostDtoAsync : EfGenericDtoAsync<Post, DetailPostDtoAsync>
    {

        [UIHint("HiddenInput")]
        [Key]
        public int PostId { get; set; }

        [MinLength(2), MaxLength(128)]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        [Required]
        public string Content { get; set; }

        //AutoMapper understands that this means Blogger.Name and fills it with the Name of the Blogger
        public string BloggerName { get; set; }

        //-------------------------------------------
        //properties that cannot be set directly (The data layer looks after them)

        [ScaffoldColumn(false)]
        [DoNotCopyBackToDatabase]
        public DateTime LastUpdated { get; set; }

        //------------------------------------------
        //these two items are altered by the SetupRestOfDto method based on the user's selection

        [UIHint("HiddenInput")]
        public int BlogId { get; set; }

        [ScaffoldColumn(false)]
        public ICollection<Tag> Tags { get; set; }            //this must be copied back

        //-------------------------------------------
        //now the various lists for user interaction

        /// <summary>
        /// This allows a single blogger to be chosen from the list
        /// </summary>
        public DropDownListType Bloggers { get; set; }

        public MultiSelectListType UserChosenTags { get; set; }

        //-------------------------------------------
        //calculated properties to help display
        //(Note: SampleMvcWebApp was written before calculated properties using [Computed] was added to GenericServices.
        //see https://github.com/JonPSmith/GenericServices/wiki/Calculated-properties for a better way of doing this

        /// <summary>
        /// When it was last updated in DateTime format
        /// </summary>
        public DateTime LastUpdatedUtc { get { return DateTime.SpecifyKind(LastUpdated, DateTimeKind.Utc); } }

        public string TagNames { get { return string.Join(", ", Tags.Select(x => x.Name)); } }

        //ctor
        public DetailPostDtoAsync()
        {
            Bloggers = new DropDownListType();
            UserChosenTags = new MultiSelectListType();
        }


        //----------------------------------------------
        //overridden methods

        protected override CrudFunctions SupportedFunctions
        {
            get { return CrudFunctions.AllCrud; }
        }

        /// <summary>
        /// This sets up the dropdownlist for the possible bloggers and the MultiSelectList of tags
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        protected override async Task SetupSecondaryDataAsync(IGenericServicesDbContext context, DetailPostDtoAsync dto)
        {

            var bloggers = await context.Set<Blog>().ToListAsync();

            dto.Bloggers.SetupDropDownListContent(bloggers.Select(x => new KeyValuePair<string, string>(x.Name, x.BlogId.ToString("D"))),
                "--- choose blogger ---");
            if (dto.PostId != 0)
                //there is an entry, so set the selected value to that
                dto.Bloggers.SetSelectedValue(dto.BlogId.ToString("D"));

            var preselectedTags = dto.PostId == 0
                ? new List<KeyValuePair<string, int>>()     //Create, so no tags selected yet
                : Tags
                    .Select(x => new { Key = x.Name, Value = x.TagId })
                    .ToList()
                    .Select(x => new KeyValuePair<string, int>(x.Key, x.Value))
                    .ToList();
            dto.UserChosenTags.SetupMultiSelectList(
                context.Set<Tag>().ToList().Select(x => new KeyValuePair<string, int>(x.Name, x.TagId)), preselectedTags);
        }


        protected override async Task<ISuccessOrErrors<Post>> CreateDataFromDtoAsync(IGenericServicesDbContext context, DetailPostDtoAsync source)
        {
            var status = await SetupRestOfDto(context);

            return status.IsValid
                ? await base.CreateDataFromDtoAsync(context, this)
                : SuccessOrErrors<Post>.ConvertNonResultStatus(status);
        }

        protected override async Task<ISuccessOrErrors> UpdateDataFromDtoAsync(IGenericServicesDbContext context, DetailPostDtoAsync source, Post destination)
        {
            var status = await SetupRestOfDto(context, destination);

            return status.IsValid
                ? await base.UpdateDataFromDtoAsync(context, this, destination)
                : status;
        }

        private async Task<ISuccessOrErrors> SetupRestOfDto(IGenericServicesDbContext context, Post post = null)
        {

            var db = context as SampleWebAppDb;
            if (db == null)
                throw new NullReferenceException("The IDbContextWithValidation must be linked to TemplateWebAppDb.");

            var status = SuccessOrErrors.Success("OK if no errors set");

            //now we sort out the blogger
            var errMsg = await SetupBloggerIdFromDropDownList(db, post);
            if (errMsg != null)
                status.AddNamedParameterError("Bloggers", errMsg);

            //now we sort out the tags
            errMsg = await ChangeTagsBasedOnMultiSelectList(db, post);
            if (errMsg != null)
                status.AddNamedParameterError("UserChosenTags", errMsg);

            return status;
        }


        //---------------------------------------------------
        //private helpers

        private async Task<string> SetupBloggerIdFromDropDownList(SampleWebAppDb db, Post post)
        {

            var blogId = Bloggers.SelectedValueAsInt;
            if (blogId == null)
                return "The blogger was not selected. You must do that before the post can be saved.";

            var blogger = await db.Blogs.FindAsync((int)blogId);
            if (blogger == null)
                return "Could not find the blogger you selected. Did another user delete it?";

            BlogId = (int)blogId;
            return null;
        }

        private async Task<string> ChangeTagsBasedOnMultiSelectList(SampleWebAppDb db, Post post)
        {
            var requiredTagIds = UserChosenTags.GetFinalSelectionAsInts();
            if (!requiredTagIds.Any())
                return "You must select at least one tag for the post.";

            if (requiredTagIds.Any(x => db.Tags.Find(x) == null))
                return "Could not find one of the tags. Did another user delete it?";

            if (post != null)
                //This is an update so we need to load the tags
                db.Entry(post).Collection(p => p.Tags).Load();

            var newTagsForPost = await db.Tags.Where(x => requiredTagIds.Contains(x.TagId)).ToListAsync();
            Tags = newTagsForPost;      //will be copied over by copyDtoToData

            return null;
        }
    }
}
