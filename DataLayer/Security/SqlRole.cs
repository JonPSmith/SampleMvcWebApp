using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DataLayer.Security
{
    public class SqlRole
    {

        public string RoleName { get; private set; }

        public IReadOnlyCollection<SqlPermission> Permissions { get; private set; }

        public SqlRole(string roleName, IReadOnlyCollection<SqlPermission> permissions)
        {
            RoleName = roleName;
            Permissions = permissions;
        }

        /// <summary>
        /// This will return a command to create the role
        /// </summary>
        /// <param name="db"></param>
        /// <returns>Sql command to create the role. Null if the role is a built-in role</returns>
        public string SqlCommandToCreateRole()
        {
            return Permissions.Any() 
                ? string.Format("CREATE ROLE {0}", RoleName) 
                : null;
        }

        /// <summary>
        /// This will return a command to drop a role
        /// </summary>
        /// <param name="db"></param>
        /// <returns>Sql command to drop the role. Null if the role is a built-in role</returns>
        public string SqlCommandToDropRole()
        {
            return Permissions.Any()
                ? string.Format("DROP ROLE {0}", RoleName)
                : null;
        }

        /// <summary>
        /// This returns a list of GRANT/DENY SQL command to add the permissions to the role
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public IEnumerable<string> SqlCommandsToAddPermissionsToRole(DbContext db)
        {
            return Permissions.Any()
                ? Permissions.Select(x => string.Format("{0} ON {1}", x.SqlCommandToAddPermission(db), RoleName))
                : new List<string>();
        }

        public override string ToString()
        {
            return string.Join("\n", SqlCommandsToAddPermissionsToRole(null));
        }
    }
}
