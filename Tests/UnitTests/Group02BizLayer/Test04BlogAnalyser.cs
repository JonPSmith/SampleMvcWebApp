#region licence
// The MIT License (MIT)
// 
// Filename: Test04BlogAnalyser.cs
// Date Created: 2014/07/11
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
