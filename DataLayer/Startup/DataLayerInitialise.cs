#region licence
// The MIT License (MIT)
// 
// Filename: DataLayerInitialise.cs
// Date Created: 2014/05/20
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
using System.Data.Entity;
using System.Linq;
using DataLayer.DataClasses;
using DataLayer.Startup.Internal;
using GenericLibsBase;
using GenericServices;

namespace DataLayer.Startup
{
    public enum TestDataSelection { Small = 0, Medium = 1}

    public static class DataLayerInitialise
    {

        private static IGenericLogger _logger;

        private static readonly Dictionary<TestDataSelection, string> XmlBlogsDataFileManifestPath = new Dictionary<TestDataSelection, string>
            {
                {TestDataSelection.Small, "DataLayer.Startup.Internal.BlogsContentSimple.xml"},
                {TestDataSelection.Medium, "DataLayer.Startup.Internal.BlogsContextMedium.xml"}
            };

        /// <summary>
        /// This should be called at Startup
        /// </summary>
        /// <param name="isAzure">true if running on azure (used for configuring retry policy and BuildSqlConnectionString UserId)</param>
        /// <param name="canCreateDatabase">true if the database provider allows the app to drop/create a database</param>
        public static void InitialiseThis(bool isAzure, bool canCreateDatabase)
        {
            EfConfiguration.IsAzure = isAzure;
            _logger = GenericLibsBaseConfig.GetLogger("DataLayerInitialise");

            //Initialiser for the database. Only used when first access is made
            if (canCreateDatabase)
                Database.SetInitializer(new CreateDatabaseIfNotExists<SampleWebAppDb>());
            else
                //This initializer will not try to change the database
                Database.SetInitializer(new NullDatabaseInitializer<SampleWebAppDb>());
        }

        public static void ResetBlogs(SampleWebAppDb context, TestDataSelection selection)
        {
            try
            {
                context.Posts.ToList().ForEach(x => context.Posts.Remove(x));
                context.Tags.ToList().ForEach(x => context.Tags.Remove(x));
                context.Blogs.ToList().ForEach(x => context.Blogs.Remove(x));
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.Critical("Exception when resetting the blogs", ex);
                throw;
            }

            var bloggers = LoadDbDataFromXml.FormBlogsWithPosts(XmlBlogsDataFileManifestPath[selection]);

            context.Blogs.AddRange(bloggers);
            var status = context.SaveChangesWithChecking();
            if (!status.IsValid)
            {
                _logger.CriticalFormat("Error when resetting courses data. Error:\n{0}",
                    string.Join(",", status.Errors));
                throw new FormatException("problem writing to database. See log.");
            }
        }

    }

}
