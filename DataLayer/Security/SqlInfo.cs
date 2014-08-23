using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using DataLayer.Security.Internal;

namespace DataLayer.Security
{
    public static class SqlInfo
    {
        /// <summary>
        /// This returns the names of all the users filtered by type, i.e. sql or windows
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sqlUser">defaults to true, i.e. SQL users. Set to false for windows users</param>
        /// <returns></returns>
        public static IEnumerable<string> GetDatabaseUsers(this DbContext db, bool sqlUser = true)
        {
            return SqlUserAndRoleRow.GetUsersAndRoleStrings(db, sqlUser).Select(x => x.UserName).Distinct();
        }

        /// <summary>
        /// This returns all the roles that have GRANT/DENY permissions on them.
        /// Note: this excludes fixed database role and any user role that links only to a fixed database roles
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static SqlRoles GetAllPermissionRoles(this DbContext db)
        {
            return new SqlRoles(SqlRoleRow.ReadRolesRows(db));
        }

        /// <summary>
        /// This returns all the users and the roles, both fixed database roles user defined permission roles, that they are in
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sqlUser">defaults to true, i.e. SQL users. Set to false for windows users</param>
        /// <returns>list of users with associated roles</returns>
        public static IEnumerable<SqlUserAndRoles> GetUsersAndTheirRoles(this DbContext db, bool sqlUser = true)
        {
            var usersAndRoleNames = SqlUserAndRoleRow.GetUsersAndRoleStrings(db, sqlUser);
            var allRoles = SqlRoleRow.ReadRolesRows(db);
            return SqlRoles.GetAllUsersAndTheirRoles(usersAndRoleNames, allRoles);
        }

        /// <summary>
        /// This will produce a list of sql commands to add the users, define the roles 
        /// with permissions and add the roles to the the users.
        /// Note: It assumes the User Login is set up as we can't do that due to no password
        /// </summary>
        /// <param name="db"></param>
        /// <param name="usersAndRoles"></param>
        /// <param name="loginPrefix">The login name is created by prepending this parameter to the database username</param>
        /// <returns></returns>
        public static IReadOnlyList<string> AddAllUsersRolesAndPermissions(this DbContext db,
            List<SqlUserAndRoles> usersAndRoles, string loginPrefix)
        {
            var result = new List<string>();

            //first we add all the users and extract distinct roles
            result.Add( "-- Add all users (note: user Logons must be already set up)");
            result.AddRange(usersAndRoles.Select(x => x.SqlCommandToAddUserToLogin(loginPrefix)));

            result.Add( "-- create each role and its permissions");
            //we group to stop roles that are used in multiple times from being declared twice
            var allRolesGroupedByName = usersAndRoles.SelectMany(x => x.UserRoles).GroupBy( x => x.RoleName);
            foreach (var roleGroup in allRolesGroupedByName)
            {
                result.Add( roleGroup.First().SqlCommandToCreateRole());
                result.AddRange(roleGroup.First().SqlCommandsToAddPermissionsToRole(db));
            }

            result.Add("-- add each user to its roles");
            result.AddRange(usersAndRoles.SelectMany(x => x.SqlCommandToAddUserToItsRoles()) );

            return result;
        }

        /// <summary>
        /// This executes a collection of sql commands in a transaction, i.e. any error rolls back the changes 
        /// It skips any lines that are empty or start with "--", i.e. the SQL comment characters
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sqlCommands"></param>
        /// <returns>null if OK. Otherwise the error string.</returns>
        public static string ExecuteSqlCommands(this DbContext db, IEnumerable<string> sqlCommands)
        {
            string result = null;
            var itemIndex = -1;
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var text in sqlCommands)
                    {
                        itemIndex++;
                        if (string.IsNullOrEmpty(text) || text.StartsWith("--")) continue;
                        db.Database.ExecuteSqlCommand(text);
                    }
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    result = string.Format("{0} at index {1}. Message = {2}", ex.GetType().Name, itemIndex, ex.Message);
                    dbContextTransaction.Rollback(); 
                }
            }

            return result;
        }

    }
}
