using System;
using DataLayer.DataClasses.Concrete;

namespace BizLayer.BlogsAnalysis.Internal
{
    internal class PostWithWordCount
    {

        public Post ActualPost { get; private set; }

        public int WordCount { get; private set; }

        public PostWithWordCount(Post actualPost)
        {
            ActualPost = actualPost;
            WordCount =
                actualPost.Content.Split(new [] {'.', '?', '!', ' ', ';', ':', ','},
                    StringSplitOptions.RemoveEmptyEntries).Length;
        }


    }
}
