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
        /// Note: this excludes sys roles and any user role that links only to a sys role
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static SqlRoles GetAllPermissionRoles(this DbContext db)
        {
            return new SqlRoles(SqlRoleRow.ReadRolesRows(db));
        }

        /// <summary>
        /// This returns all the users and the roles, sys roles or GRANT/DENY permission roles, that they are in
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

        //public static XDocument SerializeSqlUserAndRoles(IEnumerable<SqlUserAndRoles> usersAndRoles)
        //{
        //    var x = new XmlSerializer(usersAndRoles.GetType());
        //    XDocument result;
        //    using (var stream = new MemoryStream())
        //    {
        //        x.Serialize(stream, usersAndRoles);
        //        stream.Flush();
        //        result = XDocument.Load(stream);
        //    }
        //    return result;
        //}




    }
}
