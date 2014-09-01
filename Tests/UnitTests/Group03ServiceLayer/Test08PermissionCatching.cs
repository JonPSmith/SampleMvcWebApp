using System.Linq;
using System.Threading.Tasks;
using DataLayer.DataClasses;
using DataLayer.DataClasses.Concrete;
using DataLayer.Startup;
using GenericServices.Core;
using GenericServices.Services.Concrete;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests.Group03ServiceLayer
{
    class Test08PermissionCatching
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
                DataLayerInitialise.InitialiseThis(false, true);
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
                var status =
                    service.GetMany<Attendee>().Select(x => x.FullName).TryManyWithPermissionChecking();

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.Result.Count().ShouldEqual(11);
            }
        }

        [Test]
        public async Task Check02ListAttendeeNamesAsyncOk()
        {
            _userSetup.SetUser("ada");
            using (var db = new SecureSampleWebAppDb())
            {
                //SETUP
                var service = new ListService(db);

                //ATTEMPT
                var status = await 
                    service.GetMany<Attendee>().Select(x => x.FullName).TryManyWithPermissionCheckingAsync();

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.Result.Count().ShouldEqual(11);
            }
        }

        [Test]
        public void Check03ListAttendeeNamesNoAccessBad()
        {
            _userSetup.SetUser(null);
            using (var db = new SecureSampleWebAppDb())
            {
                //SETUP
                var service = new ListService(db);

                //ATTEMPT
                var status =
                    service.GetMany<Attendee>().Select(x => x.FullName).TryManyWithPermissionChecking();

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEqual("This access was not allowed.");
                status.Result.Count().ShouldEqual(0);
            }
        }

        [Test]
        public async Task Check03ListAttendeeNamesNoAccessAsyncBad()
        {
            _userSetup.SetUser(null);
            using (var db = new SecureSampleWebAppDb())
            {
                //SETUP
                var service = new ListService(db);

                //ATTEMPT
                var status = await 
                    service.GetMany<Attendee>().Select(x => x.FullName).TryManyWithPermissionCheckingAsync();

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEqual("This access was not allowed.");
                status.Result.Count().ShouldEqual(0);
            }
        }

    }
}
