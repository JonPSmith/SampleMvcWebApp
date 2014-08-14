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

        private const string XmlCoursesDataFileManifestPath = "DataLayer.Startup.Internal.CoursesContent.xml";

        private static readonly Dictionary<TestDataSelection, string> XmlBlogsDataFileManifestPath = new Dictionary<TestDataSelection, string>
            {
                {TestDataSelection.Small, "DataLayer.Startup.Internal.BlogsContentSimple.xml"},
                {TestDataSelection.Medium, "DataLayer.Startup.Internal.BlogsContextMedium.xml"}
            };

        /// <summary>
        /// This should be called at Startup
        /// </summary>
        /// <param name="isAzure"></param>
        public static void InitialiseThis(bool isAzure)
        {
            EfConfiguration.IsAzure = isAzure;
            _logger = GenericLoggerFactory.GetLogger("DataLayerInitialise");

            //Initialiser for the database. Only used when first access is made
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<SampleWebAppDb>());
        }

        public static void ResetBlogs(SampleWebAppDb context, TestDataSelection selection)
        {

            context.Posts.ToList().ForEach(x => context.Posts.Remove(x));
            context.Tags.ToList().ForEach(x => context.Tags.Remove(x));
            context.Blogs.ToList().ForEach(x => context.Blogs.Remove(x));
            context.SaveChanges();

            var bloggers = LoadDbDataFromXml.FormBlogsWithPosts(XmlBlogsDataFileManifestPath[selection]);

            context.Blogs.AddRange(bloggers);
            var status = context.SaveChangesWithValidation();
            if (!status.IsValid)
            {
                _logger.CriticalFormat("Error when resetting blogs to data selection {0}. Error:\n{1}", selection, 
                    string.Join(",", status.Errors));
                throw new FormatException("xml derived data did not load well.");
            }
        }

        public static void ResetCourses(SampleWebAppDb context)
        {

            context.Attendees.ToList().ForEach(x => context.Attendees.Remove(x));
            context.Courses.ToList().ForEach(x => context.Courses.Remove(x));
            context.SaveChanges();

            var courses = LoadDbDataFromXml.FormCoursesWithAddendees(XmlCoursesDataFileManifestPath);

            context.Courses.AddRange(courses);
            var status = context.SaveChangesWithValidation();
            if (!status.IsValid)
            {
                _logger.CriticalFormat("Error when resetting courses data. Error:\n{0}", 
                    string.Join(",", status.Errors));
                throw new FormatException("xml derived data did not load well.");
            }
        }

    }

}
