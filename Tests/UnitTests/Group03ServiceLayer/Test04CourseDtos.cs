using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using DataLayer.DataClasses;
using DataLayer.DataClasses.Concrete;
using DataLayer.Startup;
using GenericServices.Services.Concrete;
using NUnit.Framework;
using ServiceLayer.CourseServices;
using Tests.Helpers;

namespace Tests.UnitTests.Group03ServiceLayer
{
    class Test04CourseDtos
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
        public void Check01ListCoursesOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new ListService(db);

                //ATTEMPT
                var courses = service.GetList<CourseListDto>().ToList();

                //VERIFY
                courses.Count.ShouldEqual(2);
                courses[0].StartDate.ShouldEqual(new DateTime(1842 ,7, 2));
            }
        }

        [Test]
        public void Check02CourseDetailOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var firstCourseId = db.Courses.First().CourseId;
                var service = new DetailService(db);

                //ATTEMPT
                var course = service.GetDetail<CourseDetailDto>(firstCourseId);

                //VERIFY
                course.ShouldNotEqualNull();
                course.AttendeesNames.ShouldEqual("Andrew Crosse, Sir David Brewster, Charles Wheatstone, Charles Dickens, Michael Faraday, John Hobhouse");
            }
        }

        [Test]
        public void Check05CourseUpdateOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var firstCourseId = db.Courses.First().CourseId;
                var setupService = new UpdateSetupService(db);
                var service = new UpdateService(db);

                //ATTEMPT
                var dto = setupService.GetOriginal<CourseDetailDto>(firstCourseId);
                dto.Name = "Unit Test";
                dto.StartDate = new DateTime(2000,1,1);
                var status = service.Update(dto);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                var course = db.Courses.Include(x => x.Attendees).AsNoTracking().First();
                course.Name.ShouldEqual( "Unit Test");
                course.StartDate.ShouldEqual(new DateTime(2000, 1, 1));
                //not changed
                course.MainPresenter.ShouldEqual("Ada Lovelace");
                string.Join( ", ", course.Attendees.Select( x => x.FullName))
                    .ShouldEqual("Andrew Crosse, Sir David Brewster, Charles Wheatstone, Charles Dickens, Michael Faraday, John Hobhouse");
            }
        }

        [Test]
        public void Check06CourseCreateOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var setupService = new CreateSetupService(db);
                var service = new CreateService(db);

                //ATTEMPT
                var dto = setupService.GetDto<CourseDetailDto>();
                dto.Name = "Unit Test";
                dto.MainPresenter = "A person";
                dto.Description = "a description";
                dto.StartDate = new DateTime(2000, 1, 1);
                var status = service.Create(dto);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                var course =
                    db.Courses.Include(x => x.Attendees).AsNoTracking().OrderByDescending(x => x.CourseId).First();
                course.Name.ShouldEqual("Unit Test");
                course.MainPresenter.ShouldEqual("A person");
                course.StartDate.ShouldEqual(new DateTime(2000, 1, 1));
                course.Attendees.Count.ShouldEqual(0);
            }
        }

    }
}
