#region licence
// The MIT License (MIT)
// 
// Filename: LoadDbDataFromXml.cs
// Date Created: 2014/05/22
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using DataLayer.DataClasses.Concrete;

[assembly: InternalsVisibleTo("Tests")]

namespace DataLayer.Startup.Internal
{

    internal static class LoadDbDataFromXml
    {

        /// <summary>
        /// Loads the Blogs, Posts and Tags from Xml file
        /// </summary>
        /// <param name="filepathWithinAssembly"></param>
        /// <returns></returns>
        public static IEnumerable<Blog> FormBlogsWithPosts(string filepathWithinAssembly)
        {
            var assemblyHoldingFile = Assembly.GetAssembly(typeof(LoadDbDataFromXml));

            using (var fileStream = assemblyHoldingFile.GetManifestResourceStream(filepathWithinAssembly))
            {
                if (fileStream == null)
                    throw new NullReferenceException("Could not find the xml file you asked for. Did you remember to set properties->BuildAction to Embedded Resource?");
                var xmlData = XElement.Load(fileStream);
                
                //now decode and return
                var tagsDict = DecodeTags(xmlData.Element("Tags"));
                return DecodeBlogs(xmlData.Element("Blogs"), tagsDict);
            }
        }

        private static IEnumerable<Blog> DecodeBlogs(XElement element, Dictionary<string, Tag> tagsDict)
        {
            var result = new Collection<Blog>();
            foreach (var blogXml in element.Elements("Blog"))
            {
                var newBlogger = new Blog()
                {
                    Name = blogXml.Element("Name").Value,
                    EmailAddress = blogXml.Element("Email").Value,
                    Posts = new Collection<Post>()
                };

                foreach (var postXml in blogXml.Element("Posts").Elements("Post"))
                {
                    var content = postXml.Element("Content").Value;
                    var trimmedContent = string.Join("\n", content.Split('\n').Select(x => x.Trim()));
                    var newPost = new Post()
                    {
                        Blogger = newBlogger,
                        Title = postXml.Element("Title").Value,
                        Content = trimmedContent,
                        Tags = postXml.Element("TagSlugs").Value.Split(',').Select(x => tagsDict[x.Trim()]).ToList()
                    };
                    newBlogger.Posts.Add(newPost );
                }    
                result.Add( newBlogger);
            }
            return result;

        }

        private static Dictionary<string, Tag> DecodeTags(XElement element)
        {
            var result = new Dictionary<string, Tag>();
            foreach (var newTag in element.Elements("Tag").Select(tagXml => new Tag()
            {
                Name = tagXml.Element("Name").Value,
                Slug = tagXml.Element("Slug").Value
            }))
            {
                result[newTag.Slug] = newTag;
            }
            return result;
        }
    }
}
