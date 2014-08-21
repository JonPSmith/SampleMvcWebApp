using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using DataLayer.Security;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests.Group01DataLayer
{
    class Test01BuildSqlUserRoles
    {

        [Test]
        public void Test01BuildPermissionNoColumnOk()
        {
            //SETUP         

            //ATTEMPT
            var p = new SqlPermission(PermissionsOnWhat.SqlUser, PermissionStates.Grant, PermissionTypeFlags.Insert,
                "dbo", "Table", 0);

            //VERIFY
            p.ToString().ShouldEqual("SqlUser: GRANT INSERT ON OBJECT::dbo.Table");
        }

        [Test]
        public void Test02BuildPermissionSqlCommandsOk()
        {
            //SETUP         

            //ATTEMPT
            var p = new SqlPermission(PermissionsOnWhat.SqlUser, PermissionStates.Grant, 
                PermissionTypeFlags.Insert | PermissionTypeFlags.Select,
                "dbo", "Table", 0);

            //VERIFY
            p.SqlCommandToAddPermission(null).ShouldEqual("GRANT INSERT  SELECT ON OBJECT::dbo.Table");
        }


        [Test]
        public void Test05BuildPermissionWithColumnOk()
        {
            //SETUP         

            //ATTEMPT
            var p = new SqlPermission(PermissionsOnWhat.SqlUser, PermissionStates.Grant, PermissionTypeFlags.Insert,
                "dbo", "Table", 2);

            //VERIFY
            p.ToString().ShouldEqual("SqlUser: GRANT INSERT ON OBJECT::dbo.Table(Column[2])");
        }

        //-----------------------------------------

        [Test]
        public void Test10BuildRoleWithSinglePermissionOk()
        {
            //SETUP         
            var p = new SqlPermission(PermissionsOnWhat.SqlUser, PermissionStates.Grant, PermissionTypeFlags.Insert,
                "dbo", "Table", 0);

            //ATTEMPT
            var r = new SqlRole("TestRole", new Collection<SqlPermission> {p});

            //VERIFY
            r.ToString().ShouldEqual("GRANT INSERT ON OBJECT::dbo.Table ON [TestRole]");
        }

        [Test]
        public void Test11SqlCommandCreateRoleOk()
        {
            //SETUP         
            var p = new SqlPermission(PermissionsOnWhat.SqlUser, PermissionStates.Grant, PermissionTypeFlags.Insert,
                "dbo", "Table", 0);

            //ATTEMPT
            var r = new SqlRole("TestRole", new Collection<SqlPermission> { p });

            //VERIFY
            r.SqlCommandToCreateRole().ShouldEqual("CREATE ROLE [TestRole]");
        }

        [Test]
        public void Test12SqlCommandDropRoleOk()
        {
            //SETUP         
            var p = new SqlPermission(PermissionsOnWhat.SqlUser, PermissionStates.Grant, PermissionTypeFlags.Insert,
                "dbo", "Table", 0);

            //ATTEMPT
            var r = new SqlRole("TestRole", new Collection<SqlPermission> { p });

            //VERIFY
            r.SqlCommandToDropRole().ShouldEqual("DROP ROLE [TestRole]");
        }

        [Test]
        public void Test13SqlCommandAddPermissionsToRoleOk()
        {
            //SETUP         
            var p = new SqlPermission(PermissionsOnWhat.SqlUser, PermissionStates.Grant, PermissionTypeFlags.Insert,
                "dbo", "Table", 0);

            //ATTEMPT
            var r = new SqlRole("TestRole", new Collection<SqlPermission> { p });

            //VERIFY
            r.SqlCommandsToAddPermissionsToRole(null).Count().ShouldEqual(1);
            r.SqlCommandsToAddPermissionsToRole(null).First().ShouldEqual("GRANT INSERT ON OBJECT::dbo.Table ON [TestRole]");
        }

        [Test]
        public void Test15BuildRoleWithTwoPermissionsOk()
        {
            //SETUP         
            var p1 = new SqlPermission(PermissionsOnWhat.SqlUser, PermissionStates.Grant, PermissionTypeFlags.Insert,
                "dbo", "Table", 0);
            var p2 = new SqlPermission(PermissionsOnWhat.SqlUser, PermissionStates.Deny, PermissionTypeFlags.Select,
                "dbo", "Table", 0);

            //ATTEMPT
            var r = new SqlRole("TestRole", new Collection<SqlPermission> { p1, p2 });

            //VERIFY
            r.ToString().ShouldEqual("GRANT INSERT ON OBJECT::dbo.Table ON [TestRole]\nDENY SELECT ON OBJECT::dbo.Table ON [TestRole]");
        }

        [Test]
        public void Test16SqlCommandAddPermissionsToRoleOk()
        {
            //SETUP         
            var p1 = new SqlPermission(PermissionsOnWhat.SqlUser, PermissionStates.Grant, PermissionTypeFlags.Insert,
                "dbo", "Table", 0);
            var p2 = new SqlPermission(PermissionsOnWhat.SqlUser, PermissionStates.Deny, PermissionTypeFlags.Select,
                "dbo", "Table", 0);

            //ATTEMPT
            var r = new SqlRole("TestRole", new Collection<SqlPermission> { p1, p2 });

            //VERIFY
            r.SqlCommandsToAddPermissionsToRole(null).Count().ShouldEqual(2);
            r.SqlCommandsToAddPermissionsToRole(null).First().ShouldEqual("GRANT INSERT ON OBJECT::dbo.Table ON [TestRole]");
            r.SqlCommandsToAddPermissionsToRole(null).Last().ShouldEqual("DENY SELECT ON OBJECT::dbo.Table ON [TestRole]");
        }

        //----------------------------------------------
        //users

        [Test]
        public void Test20UserAddCommandOk()
        {
            //SETUP         
            var p = new SqlPermission(PermissionsOnWhat.SqlUser, PermissionStates.Grant, PermissionTypeFlags.Insert,
                "dbo", "Table", 0);
            var r = new SqlRole("TestRole", new Collection<SqlPermission> { p });

            //ATTEMPT
            var u = new SqlUserAndRoles("User", PermissionsOnWhat.SqlUser, new Collection<SqlRole> {r});

            //VERIFY
            u.SqlCommandToAddUserToLogin().ShouldEqual("CREATE USER [User] FOR LOGIN [User] WITH DEFAULT_SCHEMA=[dbo]");
        }

        [Test]
        public void Test21UserRemoveOk()
        {
            //SETUP         
            var p = new SqlPermission(PermissionsOnWhat.SqlUser, PermissionStates.Grant, PermissionTypeFlags.Insert,
                "dbo", "Table", 0);
            var r = new SqlRole("TestRole", new Collection<SqlPermission> { p });

            //ATTEMPT
            var u = new SqlUserAndRoles("User", PermissionsOnWhat.SqlUser, new Collection<SqlRole> { r });

            //VERIFY
            u.SqlCommandToRemoveUserFromLogin().ShouldEqual("DROP USER [User]");
        }

        [Test]
        public void Test22UserAddRolesOk()
        {
            //SETUP         
            var p = new SqlPermission(PermissionsOnWhat.SqlUser, PermissionStates.Grant, PermissionTypeFlags.Insert,
                "dbo", "Table", 0);
            var r = new SqlRole("TestRole", new Collection<SqlPermission> { p });

            //ATTEMPT
            var u = new SqlUserAndRoles("User", PermissionsOnWhat.SqlUser, new Collection<SqlRole> { r });

            //VERIFY
            u.SqlCommandToAddUserToItsRoles().Count().ShouldEqual(1);
            u.SqlCommandToAddUserToItsRoles().First().ShouldEqual("ALTER [TestRole] ADD MEMBER [User]");
        }

        [Test]
        public void Test23UserRemoveRolesOk()
        {
            //SETUP         
            var p = new SqlPermission(PermissionsOnWhat.SqlUser, PermissionStates.Grant, PermissionTypeFlags.Insert,
                "dbo", "Table", 0);
            var r = new SqlRole("TestRole", new Collection<SqlPermission> { p });

            //ATTEMPT
            var u = new SqlUserAndRoles("User", PermissionsOnWhat.SqlUser, new Collection<SqlRole> { r });

            //VERIFY
            u.SqlCommandToRemoveUserFromItsRoles().Count().ShouldEqual(1);
            u.SqlCommandToRemoveUserFromItsRoles().First().ShouldEqual("ALTER [TestRole] DROP MEMBER [User]");
        }

        //---------------------------------
        //Join it all together

        [Test]
        public void Test30BuildCommandsCommonRoleOk()
        {
            //SETUP  
            DbContext db = null;
            var p = new SqlPermission(PermissionsOnWhat.SqlUser, PermissionStates.Grant, PermissionTypeFlags.Insert,
                "dbo", "Table", 0);
            var r = new SqlRole("TestRole", new Collection<SqlPermission> { p });
            var u1 = new SqlUserAndRoles("User1", PermissionsOnWhat.SqlUser, new Collection<SqlRole> { r });
            var u2 = new SqlUserAndRoles("User2", PermissionsOnWhat.SqlUser, new Collection<SqlRole> { r });

            //ATTEMPT
            var list = db.AddAllUsersRolesAndPermissions(new List<SqlUserAndRoles> {u1, u2});

            //VERIFY
            list.Count.ShouldEqual(9);
            list[1].ShouldStartWith("CREATE USER [User1]");
            list[2].ShouldStartWith("CREATE USER [User2]");

            list[4].ShouldEqual("CREATE ROLE [TestRole]");
            list[5].ShouldEqual("GRANT INSERT ON OBJECT::dbo.Table ON [TestRole]");

            list[7].ShouldEqual("ALTER [TestRole] ADD MEMBER [User1]");
            list[8].ShouldEqual("ALTER [TestRole] ADD MEMBER [User2]");

        }

        [Test]
        public void Test31BuildCommandsDiffentRolesOk()
        {
            //SETUP  
            DbContext db = null;
            var p = new SqlPermission(PermissionsOnWhat.SqlUser, PermissionStates.Grant, PermissionTypeFlags.Insert,
                "dbo", "Table", 0);
            var r1 = new SqlRole("TestRole1", new Collection<SqlPermission> { p });
            var r2 = new SqlRole("TestRole2", new Collection<SqlPermission> { p });
            var u1 = new SqlUserAndRoles("User1", PermissionsOnWhat.SqlUser, new Collection<SqlRole> { r1 });
            var u2 = new SqlUserAndRoles("User2", PermissionsOnWhat.SqlUser, new Collection<SqlRole> { r2 });

            //ATTEMPT
            var list = db.AddAllUsersRolesAndPermissions(new List<SqlUserAndRoles> {u1, u2});

            //VERIFY
            list.Count.ShouldEqual(11);
            list[1].ShouldStartWith("CREATE USER [User1]");
            list[2].ShouldStartWith("CREATE USER [User2]");

            list[4].ShouldEqual("CREATE ROLE [TestRole1]");
            list[5].ShouldEqual("GRANT INSERT ON OBJECT::dbo.Table ON [TestRole1]");
            list[6].ShouldEqual("CREATE ROLE [TestRole2]");
            list[7].ShouldEqual("GRANT INSERT ON OBJECT::dbo.Table ON [TestRole2]");

            list[9].ShouldEqual("ALTER [TestRole1] ADD MEMBER [User1]");
            list[10].ShouldEqual("ALTER [TestRole2] ADD MEMBER [User2]");
        }

        [Test]
        public void Test32BuildCommandsRoleReferOtherRoleOk()
        {
            //SETUP  
            DbContext db = null;
            var p = new SqlPermission(PermissionsOnWhat.SqlUser, PermissionStates.Grant, PermissionTypeFlags.Insert,
                "dbo", "Table", 0);
            var r1 = new SqlRole("TestRole1", new Collection<SqlPermission> { p });
            var r2 = new SqlRole("db_datawriter", null);
            var u1 = new SqlUserAndRoles("User1", PermissionsOnWhat.SqlUser, new Collection<SqlRole> { r1, r2 });

            //ATTEMPT
            var list = db.AddAllUsersRolesAndPermissions(new List<SqlUserAndRoles> { u1 });

            //VERIFY
            list.Count.ShouldEqual(9);
            list[1].ShouldStartWith("CREATE USER [User1]");

            list[3].ShouldEqual("CREATE ROLE [TestRole1]");
            list[4].ShouldEqual("GRANT INSERT ON OBJECT::dbo.Table ON [TestRole1]");
            list[5].ShouldEqual("");

            list[7].ShouldEqual("ALTER [TestRole1] ADD MEMBER [User1]");
            list[8].ShouldEqual("ALTER [db_datawriter] ADD MEMBER [User1]");
        }
    }
}
