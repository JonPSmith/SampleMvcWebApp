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
        /// <param name="isAzure">true if running on azure (used for configuring retry policy and BuildSqlConnectionString UserId)</param>
        /// <param name="canCreateDatabase">true if the database provider allows the app to drop/create a database</param>
        public static void InitialiseThis(bool isAzure, bool canCreateDatabase)
        {
            EfConfiguration.IsAzure = isAzure;
            _logger = ServicesConfiguration.GetLogger("DataLayerInitialise");



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

        public static void ResetCourses(SampleWebAppDb context)
        {
            try
            {
                context.Attendees.ToList().ForEach(x => context.Attendees.Remove(x));
                context.Courses.ToList().ForEach(x => context.Courses.Remove(x));
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.Critical("Exception when resetting the courses", ex);
                throw;
            }

            var courses = LoadDbDataFromXml.FormCoursesWithAddendees(XmlCoursesDataFileManifestPath);

            context.Courses.AddRange(courses);
            var status = context.SaveChangesWithChecking();
            if (!status.IsValid)
            {
                _logger.CriticalFormat("Error when resetting courses data. Error:\n{0}", 
                    string.Join(",", status.Errors));
                throw new FormatException("xml derived data did not load well. See log.");
            }
        }

    }

}
