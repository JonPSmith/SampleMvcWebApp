using System.Data.SqlClient;
using System.Linq;
using DataLayer.DataClasses;
using DataLayer.Startup;
using GenericServices.Core;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests.Group01DataLayer
{
    class Test05SecureDb
    {
        private ClaimsIdentityHelper _userSetup;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            using (var db = new SampleWebAppDb())
            {
                DataLayerInitialise.InitialiseThis(false);
                DataLayerInitialise.ResetBlogs(db, TestDataSelection.Small);
                DataLayerInitialise.ResetCourses(db);
            }
            _userSetup = new ClaimsIdentityHelper();
        }

        [Test]
        public void Test01NoSecuirityOk()
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

        [Test]
        public void Test05SetSecurityBadThrowsException()
        {
            _userSetup.SetUser("bad");
            using (var db = new SecureSampleWebAppDb())
            {
                //SETUP
                DataLayerInitialise.InitialiseThis(false);

                //ATTEMPT
                var ex = Assert.Throws<SqlException>(() => DataLayerInitialise.ResetCourses(db));

                //VERIFY
                ex.Message.ShouldEqual("Login failed for user 'BadUser'.");
            }
        }

        [Test]
        public void Test06ReadOk()
        {
            //SETUP
            _userSetup.SetUser("ada");
            using (var db = new SecureSampleWebAppDb())
            {

                //ATTEMPT

                //VERIFY
                db.Blogs.Count().ShouldEqual(2);
            }
        }

        [Test]
        public void Test10SetUserHasSecurityOk()
        {
            //SETUP
            _userSetup.SetUser("ada");
            using (var db = new SecureSampleWebAppDb())
            {
                DataLayerInitialise.InitialiseThis(false);

                //ATTEMPT
                DataLayerInitialise.ResetCourses(db);

                //VERIFY
                db.Courses.Count().ShouldEqual(2);
                db.Attendees.Count().ShouldEqual(11);
            }
        }

        [Test]
        public void Test11SetUserDoesNotHasSecurityBad()
        {
            //SETUP
            _userSetup.SetUser("michael");
            using (var db = new SecureSampleWebAppDb())
            {
                DataLayerInitialise.InitialiseThis(false);

                //ATTEMPT
                var ex = Assert.Throws<System.Data.Entity.Core.EntityCommandExecutionException>(() => DataLayerInitialise.ResetCourses(db));

                //VERIFY
                ex.Message.ShouldEqual("An error occurred while executing the command definition. See the inner exception for details.");
            }
        }

        //----------------------------------------------------------------
        //now test the HasPaid flag: write 

        [Test]
        public void Test20CanChangeHasPaidOk()
        {
            //SETUP
            _userSetup.SetUser("william");
            using (var db = new SecureSampleWebAppDb())
            {
                var attendee = db.Attendees.First();
                var newHasPaid = !attendee.HasPaid;

                //ATTEMPT
                attendee.HasPaid = newHasPaid;
                var status = db.SaveChangesWithValidation();

                //VERIFY
                status.IsValid.ShouldEqual(true);
                var reload = db.Attendees.First();
                reload.HasPaid.ShouldEqual(newHasPaid);
            }
        }

        [Test]
        public void Test21CannotChangeHasPaidBad()
        {
            //SETUP
            _userSetup.SetUser("ada");
            using (var db = new SecureSampleWebAppDb())
            {
                var attendee = db.Attendees.First();

                //ATTEMPT
                attendee.HasPaid = !attendee.HasPaid;
                var status = db.SaveChangesWithValidation();

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEqual("You are not allowed to change that item.");
            }
        }

        //----------------------------------------------------
        //HasPaid: read

        [Test]
        public void Test25CanReadAttendeeHasPaidOk()
        {
            //SETUP
            _userSetup.SetUser("ada");
            using (var db = new SecureSampleWebAppDb())
            {

                //ATTEMPT
                var info = db.Attendees.First();

                //VERIFY
                info.ShouldNotEqualNull();
            }
        }

        [Test]
        public void Test26CannotReadAttendeeHasPaidBad()
        {
            //SETUP
            _userSetup.SetUser("michael");
            using (var db = new SecureSampleWebAppDb())
            {

                //ATTEMPT
                var ex = Assert.Throws<System.Data.Entity.Core.EntityCommandExecutionException>(()
                    => db.Attendees.First());

                //VERIFY

            }
        }

        [Test]
        public void Test27CannotReadAttendeeHasPaidBadWithHelper()
        {
            //SETUP
            _userSetup.SetUser("michael");
            using (var db = new SecureSampleWebAppDb())
            {

                //ATTEMPT
                var status = db.Attendees.TryManyWithPermissionChecking();

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEqual("This access was not allowed.");
                status.Result.Count().ShouldEqual(0);
            }
        }

        //----------------------------------------------------------------
        //can/cannot read attendees

        [Test]
        public void Test30CanReadAttendessOk()
        {
            //SETUP
            _userSetup.SetUser("charles");
            using (var db = new SecureSampleWebAppDb())
            {

                //ATTEMPT
                var info = db.Courses.Select( x => new { Course = x, Names = x.Attendees.Select( y => y.FullName) }).First();

                //VERIFY
                info.Names.Count().ShouldEqual(6);
            }
        }

        [Test]
        public void Test31CannotReadAttendeesBad()
        {
            //SETUP
            _userSetup.SetUser(null);
            using (var db = new SecureSampleWebAppDb())
            {

                //ATTEMPT
                var ex = Assert.Throws<System.Data.Entity.Core.EntityCommandExecutionException>(() 
                    => db.Courses.Select(x => new { Course = x, Names = x.Attendees.Select(y => y.FullName) }).First());

                //VERIFY

            }
        }

    }
}
