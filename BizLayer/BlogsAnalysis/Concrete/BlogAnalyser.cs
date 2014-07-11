using System;
using System.Data.Entity;
using System.Linq;
using DataLayer.DataClasses;
using GenericServices;
using GenericServices.Actions;
using GenericServices.Core;

namespace BizLayer.BlogsAnalysis.Concrete
{
    public class BlogAnalyser : ActionBase, IBlogAnalyser
    {

        private readonly SampleWebAppDb _db;

        /// <summary>
        /// If true then the caller should call EF SubmitChanges if the method exited with status IsValid and
        /// it looks to see if the data part has a ICheckIfWarnings and if the WriteEvenIfWarning is false
        /// and there are warnings then it does not call SubmitChanges
        /// </summary>
        public override bool SubmitChangesOnSuccess { get { return false; } }

        public BlogAnalyser(SampleWebAppDb db)
        {
            _db = db;
        }

        public ISuccessOrErrors<AnalysisResult> DoAction(int blogId)
        {
            if (blogId == 0)
                throw new ArgumentException("The blogId cannot be zero.");

            ISuccessOrErrors<AnalysisResult> status = new SuccessOrErrors<AnalysisResult>();

            var bloggerWithPostsAndTags =
                _db.Blogs.Where(x => x.BlogId == blogId).Include(x => x.Posts.Select(y => y.Tags)).AsNoTracking().SingleOrDefault();

            if (bloggerWithPostsAndTags == null)
                return
                    status.AddSingleError("Could not find the blogger reference you asked for. Have they been deleted?");

            return status.SetSuccessWithResult(new AnalysisResult(bloggerWithPostsAndTags), "Analysis of blogger '{0}",
                bloggerWithPostsAndTags.Name);

        }

    }
}
