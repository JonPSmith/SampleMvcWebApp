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
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new ListService(db);

                //ATTEMPT
                var attendees = service.GetList<Attendee>().Select( x => x.FullName).ToList();

                //VERIFY
                attendees.Count.ShouldEqual(11);
            }
        }

        [Test]
        public void Check02AttendeeDetailOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var firstId = db.Attendees.First().AttendeeId;
                var service = new DetailService(db);

                //ATTEMPT
                var attendee = service.GetDetail<AttendeeDetailAllDto>(firstId);

                //VERIFY
                attendee.ShouldNotEqualNull();
                attendee.FullName.ShouldEqual("Andrew Crosse");
            }
        }

        [Test]
        public void Check05AttendeeUpdateAllOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var firstAttendeeId = db.Attendees.First().AttendeeId;
                var setupService = new UpdateSetupService(db);
                var service = new UpdateService(db);

                //ATTEMPT
                var dto = setupService.GetOriginal<AttendeeDetailAllDto>(firstAttendeeId);
                dto.FullName = "Unit Test";
                dto.EmailAddress = "new@nospam.com";
                dto.HasPaid = false;
                var status = service.Update(dto);

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
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var firstAttendeeId = db.Attendees.First().AttendeeId;
                var setupService = new UpdateSetupService(db);
                var service = new UpdateService(db);

                //ATTEMPT
                var dto = setupService.GetOriginal<AttendeeNotPaidDto>(firstAttendeeId);
                dto.FullName = "Unit Test";
                dto.EmailAddress = "new@nospam.com";
                var status = service.Update(dto);

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
            using (var db = new SampleWebAppDb())
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
