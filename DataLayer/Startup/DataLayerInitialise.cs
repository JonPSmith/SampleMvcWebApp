using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DataLayer.DataClasses;
using DataLayer.Startup.Internal;
using GenericServices;
using GenericServices.Logger;

namespace DataLayer.Startup
{
    public enum TestDataSelection { Small = 0, Medium = 1}

    public static class DataLayerInitialise
    {

        private static IGenericLogger _logger;

        private static readonly Dictionary<TestDataSelection, string> XmlDataFileManifestPath = new Dictionary<TestDataSelection, string>
            {
                {TestDataSelection.Small, "DataLayer.Startup.Internal.DbContentSimple.xml"},
                {TestDataSelection.Medium, "DataLayer.Startup.Internal.DbContextMedium.xml"}
            };

        /// <summary>
        /// This should be called at Startup
        /// </summary>
        public static void InitialiseThis()
        {
            _logger = GenericLoggerFactory.GetLogger("DataLayerInitialise");
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
            var status = context.SaveChangesWithValidation();
            if (!status.IsValid)
            {
                _logger.CriticalFormat("Error when resetting database to data selection {0}. Error:\n{1}", selection, 
                    string.Join(",", status.Errors));
                throw new FormatException("xml derived data did not load well.");
            }
        }
    }

}
