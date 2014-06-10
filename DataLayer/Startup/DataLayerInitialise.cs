using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DataLayer.DataClasses;
using DataLayer.DataClasses.Concrete;
using DataLayer.Startup.Internal;

namespace DataLayer.Startup
{
    public enum TestDataSelection { Simple = 0}

    public static class DataLayerInitialise
    {

        private static readonly Dictionary<TestDataSelection, string> XmlDataFileManifestPath = new Dictionary<TestDataSelection, string>
            {
                {TestDataSelection.Simple, "DataLayer.Startup.Internal.DbContentSimple.xml"}
            };

        /// <summary>
        /// This should be called at Startup
        /// </summary>
        public static void InitialiseThis()
        {
            //Initialise the database
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<SampleWebAppDb>());
        }

        public static void ResetDatabaseToTestData(SampleWebAppDb context, TestDataSelection selection)
        {

            context.Posts.ToList().ForEach(x => context.Posts.Remove(x));
            context.Tags.ToList().ForEach(x => context.Tags.Remove(x));
            context.Blogs.ToList().ForEach(x => context.Blogs.Remove(x));
            context.SaveChanges();

            var bloggers = LoadDbDataFromXml.FormBlogsWithPosts(XmlDataFileManifestPath[selection]);

            context.Blogs.AddRange(bloggers);
            context.SaveChanges();
        }
    }

}
