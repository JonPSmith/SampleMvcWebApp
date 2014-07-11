using System.Collections.ObjectModel;
using System.Linq;
using BizLayer.BlogsAnalysis.Internal;
using DataLayer.DataClasses.Concrete;

namespace BizLayer.BlogsAnalysis.Concrete
{
    public class AnalysisResult
    {

        public string BloggerName { get; private set; }

        public int NumPosts { get; private set; }

        public Post LongestPost { get; private set; }

        public int LongestWordCount { get; private set; }

        public Post ShortestPost { get; private set; }

        public int ShortestWordCount { get; private set; }

        public int AverageWordCount { get; private set; }

        public ReadOnlyCollection<Tag> TagsUsed { get; private set; }

        internal AnalysisResult(Blog bloggerWithPostsAndTags)
        {

            var postWithWordCount = bloggerWithPostsAndTags.Posts.Select(x => new PostWithWordCount(x)).OrderBy( x => x.WordCount).ToList();

            BloggerName = bloggerWithPostsAndTags.Name;
            NumPosts = bloggerWithPostsAndTags.Posts.Count;

            LongestPost = postWithWordCount.Last().ActualPost;
            LongestWordCount = postWithWordCount.Last().WordCount;
            ShortestPost = postWithWordCount.First().ActualPost;
            ShortestWordCount = postWithWordCount.First().WordCount;

            AverageWordCount = (int)postWithWordCount.Average(x => x.WordCount);
            TagsUsed = bloggerWithPostsAndTags.Posts.SelectMany(x => x.Tags).Distinct().ToList().AsReadOnly();

        }
    }
}
