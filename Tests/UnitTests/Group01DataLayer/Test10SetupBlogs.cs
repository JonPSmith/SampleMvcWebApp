using System;
using System.Linq;
using DataLayer.DataClasses;
using DataLayer.Startup;
using DataLayer.Startup.Internal;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests.Group01DataLayer
{
    class Test10SetupBlogs
    {
        [Test]
        public void Check01XmlFileLoadOk()
        {

            //SETUP

            //ATTEMPT
            var bloggers = LoadDbDataFromXml.FormBlogsWithPosts("DataLayer.Startup.Internal.BlogsContentSimple.xml").ToList();

            //VERIFY
            bloggers.Count().ShouldEqual(2);
            bloggers.SelectMany( x => x.Posts).Count().ShouldEqual(3);
            bloggers.SelectMany( x => x.Posts.SelectMany( y => y.Tags)).Distinct().Count().ShouldEqual(3);

        }

        [Test]
        public void Check02XmlFileLoadBad()
        {

            //SETUP

            //ATTEMPT
            var ex = Assert.Throws<NullReferenceException>( () => LoadDbDataFromXml.FormBlogsWithPosts("badname.xml"));

            //VERIFY
            ex.Message.ShouldStartWith("Could not find the xml file you asked for.");

        }

        [Test]
        public void Check10BlogsResetSmallOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                DataLayerInitialise.InitialiseThis(false);

                //ATTEMPT
                DataLayerInitialise.ResetBlogs(db, TestDataSelection.Small);

                //VERIFY
                db.Blogs.Count().ShouldEqual(2);
                db.Posts.Count().ShouldEqual(3);
                db.Tags.Count().ShouldEqual(3);
            }
        }

        [Test]
        public void Check11BlogsResetMediumOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                DataLayerInitialise.InitialiseThis(false);

                //ATTEMPT
                DataLayerInitialise.ResetBlogs(db, TestDataSelection.Medium);

                //VERIFY
                db.Blogs.Count().ShouldEqual(4);
                db.Posts.Count().ShouldEqual(17);
                db.Tags.Count().ShouldEqual(8);
            }
        }

    }
}
