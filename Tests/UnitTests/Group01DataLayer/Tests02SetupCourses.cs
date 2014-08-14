using System;
using System.Linq;
using DataLayer.DataClasses;
using DataLayer.Startup;
using DataLayer.Startup.Internal;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests.Group01DataLayer
{
    class Tests02SetupCourses
    {
        [Test]
        public void Check01XmlFileLoadOk()
        {

            //SETUP

            //ATTEMPT
            var courses = LoadDbDataFromXml.FormCoursesWithAddendees("DataLayer.Startup.Internal.CoursesContent.xml").ToList();

            //VERIFY
            courses.Count().ShouldEqual(2);
            courses[0].Attendees.Count.ShouldEqual(6);
            courses[1].Attendees.Count.ShouldEqual(5);
        }

        [Test]
        public void Check02XmlFileLoadBad()
        {

            //SETUP

            //ATTEMPT
            var ex = Assert.Throws<NullReferenceException>(() => LoadDbDataFromXml.FormCoursesWithAddendees("badname.xml"));

            //VERIFY
            ex.Message.ShouldStartWith("Could not find the xml file you asked for.");

        }

        [Test]
        public void Check10CoursesResetSmallOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                DataLayerInitialise.InitialiseThis(false);

                //ATTEMPT
                DataLayerInitialise.ResetCourses(db);

                //VERIFY
                db.Courses.Count().ShouldEqual(2);
                db.Attendees.Count().ShouldEqual(11);
            }
        }

    }
}
