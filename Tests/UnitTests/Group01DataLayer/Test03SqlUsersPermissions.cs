using System;
using System.IO;
using System.Linq;
using DataLayer.DataClasses;
using DataLayer.Security;
using DataLayer.Security.Internal;
using NUnit.Framework;
using SampleWebApp.Infrastructure;
using Tests.Helpers;

namespace Tests.UnitTests.Group01DataLayer
{
    class Test03SqlUsersPermissions
    {
        [Test]
        public void Test01ReadWindowsUsers()
        {
            //SETUP
            using (var db = new SampleWebAppDb())
            {

                //ATTEMPT
                var users = db.GetDatabaseUsers(false).ToList();

                //VERIFY
                Assert.Greater(users.Count, 0);
                users.Contains("dbo").ShouldEqual(true);
            }
        }

        [Test]
        public void Test02ReadSqlUsers()
        {
            //SETUP
            using (var db = new SampleWebAppDb())
            {

                //ATTEMPT
                var users = db.GetDatabaseUsers().ToList();

                //VERIFY
                Assert.Greater(users.Count, 0);
                users.Contains("dbo").ShouldEqual(false);
            }
        }

        [Test]
        public void Test10ReadRolesRawOk()
        {
            //SETUP
            using (var db = new SampleWebAppDb())
            {

                //ATTEMPT
                var roleData = SqlRoleRow.ReadRolesRows(db).ToList();

                //VERIFY
                Assert.Greater(roleData.Count, 0);
            }
        }

        [Test]
        public void Test11ReadCreateRolesOk()
        {
            //SETUP
            using (var db = new SampleWebAppDb())
            {

                //ATTEMPT
                var roles = db.GetAllPermissionRoles();

                //VERIFY
                Assert.Greater(roles.Roles.Count(), 0);
            }
        }


        [Test]
        public void Test20ReadUsersWithRolesOk()
        {
            //SETUP
            using (var db = new SampleWebAppDb())
            {

                //ATTEMPT
                var usersAndRoles = db.GetUsersAndTheirRoles().ToList();

                //VERIFY
                Assert.Greater(usersAndRoles.Count(), 0);
            }
        }

        //-----------------------------------------------------------

        [Test]
        [Ignore("Just used to check the setup of the permissions")]
        public void Test98ListDatabasePermissionsOk()
        {
            //SETUP
            const string loginPrefix = "";
            using (var db = new SampleWebAppDb())
            {

                //ATTEMPT
                var data = db.SqlCommandsCreateUsersRolesAndPermissions(loginPrefix);

                //VERIFY
                foreach (var line in data)
                    Console.WriteLine(line);
            }
        }

        [Test]
        [Ignore("Just used to check the setup of the permissions")]
        public void Test99WriteDataToAppDataOk()
        {
            //SETUP
            var data = SqlSecurityHelper.SaveSqlSecuritySetup(HostTypes.Azure, "Azure");

            //VERIFY
            foreach (var line in data)
                Console.WriteLine(line);    
        }
    }
}
