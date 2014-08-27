using System.Linq;
using DataLayer.DataClasses;
using DataLayer.DataClasses.Concrete;
using DataLayer.Startup;
using GenericServices.Services.Concrete;
using NUnit.Framework;
using ServiceLayer.AttendeeServices;
using Tests.Helpers;

namespace Tests.UnitTests.Group03ServiceLayer
{
    class Test06AttendeeDtos
    {
        private ClaimsIdentityHelper _userSetup;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _userSetup = new ClaimsIdentityHelper();
        }

        [SetUp]
        public void SetUp()
        {
            using (var db = new SampleWebAppDb())
            {
                DataLayerInitialise.InitialiseThis(false);
                DataLayerInitialise.ResetCourses(db);
            }
        }

        [Test]
        public void Check01ListAttendeeNamesOk()
        {
            _userSetup.SetUser("ada");
            using (var db = new SecureSampleWebAppDb())
            {
                //SETUP
                var service = new ListService(db);

                //ATTEMPT
                var attendees = service.GetMany<Attendee>().Select( x => x.FullName).ToList();

                //VERIFY
                attendees.Count.ShouldEqual(11);
            }
        }

        [Test]
        public void Check02AttendeeDetailOk()
        {
            _userSetup.SetUser("ada");
            using (var db = new SecureSampleWebAppDb())
            {
                //SETUP
                var firstId = db.Attendees.First().AttendeeId;
                var service = new DetailService(db);

                //ATTEMPT
                var status = service.GetDetail<AttendeeDetailAllDto>(firstId);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.Result.ShouldNotEqualNull();
                status.Result.FullName.ShouldEqual("Andrew Crosse");
            }
        }

        [Test]
        public void Check05AttendeeUpdateAllOk()
        {
            _userSetup.SetUser("william");
            using (var db = new SecureSampleWebAppDb())
            {
                //SETUP
                var firstAttendeeId = db.Attendees.First().AttendeeId;
                var setupService = new UpdateSetupService(db);
                var service = new UpdateService(db);

                //ATTEMPT
                var setupStatus = setupService.GetOriginal<AttendeeDetailAllDto>(firstAttendeeId);
                setupStatus.IsValid.ShouldEqual(true, setupStatus.Errors);
                setupStatus.Result.FullName = "Unit Test";
                setupStatus.Result.EmailAddress = "new@nospam.com";
                setupStatus.Result.HasPaid = false;
                var status = service.Update(setupStatus.Result);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                var attendee = db.Attendees.AsNoTracking().First();
                attendee.FullName.ShouldEqual("Unit Test");
                attendee.EmailAddress.ShouldEqual("new@nospam.com");
                attendee.HasPaid.ShouldEqual(false);
            }
        }

        [Test]
        public void Check06AttendeeUpdateExcludeNotPaidOk()
        {
            _userSetup.SetUser("ada");
            using (var db = new SecureSampleWebAppDb())
            {
                //SETUP
                var firstAttendeeId = db.Attendees.First().AttendeeId;
                var setupService = new UpdateSetupService(db);
                var service = new UpdateService(db);

                //ATTEMPT
                var setupStatus = setupService.GetOriginal<AttendeeNotPaidDto>(firstAttendeeId);
                setupStatus.IsValid.ShouldEqual(true, setupStatus.Errors);
                setupStatus.Result.FullName = "Unit Test";
                setupStatus.Result.EmailAddress = "new@nospam.com";
                var status = service.Update(setupStatus.Result);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                var attendee = db.Attendees.AsNoTracking().First();
                attendee.FullName.ShouldEqual("Unit Test");
                attendee.EmailAddress.ShouldEqual("new@nospam.com");
                attendee.HasPaid.ShouldEqual(true);
            }
        }

        [Test]
        public void Check10AttendeeDeleteOk()
        {
            _userSetup.SetUser("ada");
            using (var db = new SecureSampleWebAppDb())
            {
                //SETUP
                var firstAttendeeId = db.Attendees.First().AttendeeId;
                var numAttendees = db.Attendees.Count();
                var service = new DeleteService(db);

                //ATTEMPT
                var status = service.Delete<Attendee>(firstAttendeeId);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                db.Attendees.Count().ShouldEqual( numAttendees - 1);
            }
        }

    }
}
