#region licence
// The MIT License (MIT)
// 
// Filename: BlogAnalyser.cs
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
