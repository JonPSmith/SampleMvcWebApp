using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.DataClasses;
using DataLayer.DataParts;
using DataLayer.Startup;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests.Group01DataLayer
{
    class Test01SqlSecurity
    {

        [Test]
        public void Check01DefaultSecurityOpenOk()
        {
            using (var db = new SecureSampleWebAppDb())
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
        public void Check05SetSecurityBadThrowsException()
        {
            SqlSecurity.SetupUnauthenticatedDatabaseUser("BadUserName", "BadPassword");
            using (var db = new SecureSampleWebAppDb())
            {
                //SETUP
                DataLayerInitialise.InitialiseThis(false);

                //ATTEMPT
                var ex = Assert.Throws<System.Data.SqlClient.SqlException>(() => DataLayerInitialise.ResetCourses(db));

                //VERIFY
                ex.Message.ShouldEqual("Login failed for user 'BadUserName'.");
            }
        }

        [Test]
        public void Check10SetSecurityGoodOk()
        {
            //SETUP
            var baseConnection =
                System.Configuration.ConfigurationManager.ConnectionStrings[SampleWebAppDb.NameOfConnectionString].ConnectionString;
            var sb = new SqlConnectionStringBuilder(baseConnection);
            SqlSecurity.SetupUnauthenticatedDatabaseUser(sb.UserID, sb.Password);

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

    }
}
