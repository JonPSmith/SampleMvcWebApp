using System.Linq;
using BizLayer.BlogsAnalysis.Concrete;
using DataLayer.DataClasses;
using DataLayer.Startup;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests.Group02BizLayer
{
    class Test04BlogAnalyser
    {

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            using (var db = new SampleWebAppDb())
            {
                DataLayerInitialise.InitialiseThis(false, true);
                DataLayerInitialise.ResetBlogs(db, TestDataSelection.Small);
            }
        }

        [Test]
        public void Check01AnalyseRunsOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new BlogAnalyser(db);
                var blogId = db.Blogs.AsNoTracking().First().BlogId;

                //ATTEMPT
                var status = service.DoAction(blogId);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
            }
        }


        [Test]
        public void Check02AnalyseResultOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new BlogAnalyser(db);
                var firstBlogger = db.Blogs.AsNoTracking().First();

                //ATTEMPT
                var status = service.DoAction(firstBlogger.BlogId);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.Result.BloggerName.ShouldEqual(firstBlogger.Name);
                status.Result.NumPosts.ShouldEqual(2);
                status.Result.LongestWordCount.ShouldEqual(13);
                status.Result.ShortestWordCount.ShouldEqual(8);
                status.Result.AverageWordCount.ShouldEqual(10);
                status.Result.TagsUsed.Count.ShouldEqual(3);
            }
        }

        [Test]
        public void Check03AnalyseFail()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new BlogAnalyser(db);

                //ATTEMPT
                var status = service.DoAction(-1);

                //VERIFY
                status.IsValid.ShouldEqual(false, status.Errors);
            }
        }

    }
}
