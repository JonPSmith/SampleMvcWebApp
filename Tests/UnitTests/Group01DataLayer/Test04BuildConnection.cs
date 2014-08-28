using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.DataClasses;
using DataLayer.Security;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests.Group01DataLayer
{
    class Test04BuildConnection
    {
        private ClaimsIdentityHelper _userSetup;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _userSetup = new ClaimsIdentityHelper();
        }

        [SetUp]
        public void Setup()
        {
            SqlSecure.SecurityEnabled = true;
        }

        [Test]
        public void Test01CheckConnectionOk()
        {
            //SETUP


            //ATTEMPT
            var con = System.Configuration.ConfigurationManager.ConnectionStrings[SampleWebAppDb.NameOfConnectionString].ConnectionString;

            //VERIFY
            string.IsNullOrEmpty(con).ShouldEqual(false);
            var sc = new SqlConnectionStringBuilder(con);
            sc.InitialCatalog.ShouldEqual("TestSampleWebAppDb");
            sc.IntegratedSecurity.ShouldEqual(true);
        }

        [Test]
        public void Test02CheckConnectionDummyAzureOk()
        {
            //SETUP


            //ATTEMPT
            var con = System.Configuration.ConfigurationManager.ConnectionStrings["DummyAzureDb"].ConnectionString;

            //VERIFY
            string.IsNullOrEmpty(con).ShouldEqual(false);
            var sc = new SqlConnectionStringBuilder(con);
            sc.InitialCatalog.ShouldEqual("AzureDatabaseName");
            sc.IntegratedSecurity.ShouldEqual(false);
        }

        [Test]
        public void Test03BaseConnectionOk()
        {
            //SETUP
            SqlSecure.SecurityEnabled = false;
            var con = System.Configuration.ConfigurationManager.ConnectionStrings[SampleWebAppDb.NameOfConnectionString].ConnectionString;

            //ATTEMPT
            var newCon = SqlSecure.BuildSqlConnectionString(SampleWebAppDb.NameOfConnectionString, false);

            //VERIFY
            newCon.ShouldEqual(con);

        }

        [Test]
        public void Test05SetUnathenticatedUserNonAzureOk()
        {
            //SETUP
            _userSetup.SetUser(null);

            //ATTEMPT
            var con = SqlSecure.BuildSqlConnectionString(SampleWebAppDb.NameOfConnectionString, false);

            //VERIFY
            var sc = new SqlConnectionStringBuilder(con);
            sc.UserID.ShouldEqual("Homer");
            sc.IntegratedSecurity.ShouldEqual(false);
        }

        [Test]
        public void Test06SetAdaUserNonAzureOk()
        {
            //SETUP
            _userSetup.SetUser("ada");

            //ATTEMPT
            var con = SqlSecure.BuildSqlConnectionString(SampleWebAppDb.NameOfConnectionString, false);

            //VERIFY
            var sc = new SqlConnectionStringBuilder(con);
            sc.UserID.ShouldEqual("Ada");
            sc.IntegratedSecurity.ShouldEqual(false);
        }

        [Test]
        public void Test10SetUnathenticatedUserAzureOk()
        {
            //SETUP
            _userSetup.SetUser(null);

            //ATTEMPT
            var con = SqlSecure.BuildSqlConnectionString("DummyAzureDb", true);

            //VERIFY
            var sc = new SqlConnectionStringBuilder(con);
            sc.UserID.ShouldEqual("Homer@AzureServerName");
            sc.IntegratedSecurity.ShouldEqual(false);
        }
    }
}
