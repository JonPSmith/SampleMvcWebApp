using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DataLayer.Security.Internal;

namespace DataLayer.Security
{
    public class SqlRoles
    {

        public IReadOnlyCollection<SqlRole> Roles { get; private set; }

        //ctor
        internal SqlRoles(IEnumerable<SqlRoleRow> rolesRows)
        {
            Roles = BuildPrivateRoles(rolesRows).Select(x => new SqlRole(x.Key, x.Value)).ToList();
        }

        internal static IEnumerable<SqlUserAndRoles> GetAllUsersAndTheirRoles (
            IEnumerable<SqlUserAndRoleRow> userAndRoleStrings, IEnumerable<SqlRoleRow> rolesRows)
        {

            var allRoles = BuildPrivateRoles(rolesRows);
            var userTypeDict = new Dictionary<string, PermissionsOnWhat>();
            var userRoleDict = new Dictionary<string, Collection<SqlRole>>();
            foreach (var uAndR in userAndRoleStrings)
            {
                userTypeDict[uAndR.UserName] = SqlPermission.ConvertDataBasePrincipalType(uAndR.UserType);
                var sqlRole = GetRoles(uAndR.RoleName, allRoles);
                if (userRoleDict.ContainsKey(uAndR.UserName))
                    userRoleDict[uAndR.UserName].Add(sqlRole);
                else
                    userRoleDict[uAndR.UserName] = new Collection<SqlRole> {sqlRole};
            }

            return userRoleDict.Select(x => new SqlUserAndRoles(x.Key, userTypeDict[x.Key], x.Value));
        }

        private static SqlRole GetRoles(string roleName, Dictionary<string, Collection<SqlPermission>> allRoles)
        {
            return allRoles.ContainsKey(roleName)
                ? new SqlRole(roleName, allRoles[roleName])
                : new SqlRole(roleName, new Collection<SqlPermission>());
        }


        private static Dictionary<string, Collection<SqlPermission>> BuildPrivateRoles(IEnumerable<SqlRoleRow> rolesRows)
        {
            SqlPermission last = null;
            string lastRoleName = null;
            var result = new Dictionary<string, Collection<SqlPermission>>();
            foreach (var roleRow in rolesRows)
            {
                var roleName = roleRow.DatabasePrincipalsName;
                var permission = new SqlPermission(roleRow);
                //we try and combine the roles. This relies on the list being sorted to place 
                //permissions for the same permission, scheme and object next to each other
                if (roleName == lastRoleName && permission.TryCombine(last)) continue;          //if can combine then don't need to add
                last = permission;
                lastRoleName = roleName;
                if (result.ContainsKey(roleName))
                    result[roleName].Add(permission);
                else
                    result[roleName] = new Collection<SqlPermission> { permission };
            }
            return result;
        }



    }
}
