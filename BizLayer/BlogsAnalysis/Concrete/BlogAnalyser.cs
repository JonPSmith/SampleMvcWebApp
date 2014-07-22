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

            return status.SetSuccessWithResult(new AnalysisResult(bloggerWithPostsAndTags), "Analysis of blogger '{0}'.",
                bloggerWithPostsAndTags.Name);

        }

    }
}
