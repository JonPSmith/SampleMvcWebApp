using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;

namespace DataLayer.Security
{
    public class SqlRole
    {

        public string PrincipalName { get; private set; }

        public IReadOnlyCollection<SqlPermission> Permissions { get; private set; }

        public SqlRole(string principalName, IReadOnlyCollection<SqlPermission> permissions)
        {
            PrincipalName = principalName;
            Permissions = permissions ?? new List<SqlPermission>();
        }

        /// <summary>
        /// This will return a command to create the role
        /// </summary>
        /// <returns>Sql command to create the role. Empty if the role is a built-in role or permission isn't a database role</returns>
        public string SqlCommandToCreateRole()
        {
            return Permissions.Any( x => x.OnWhat == PermissionsOnWhat.DatabaseRole)
                ? string.Format("CREATE ROLE [{0}]", PrincipalName) 
                : string.Empty;
        }

        /// <summary>
        /// This will return a command to drop a role
        /// </summary>
        /// <returns>Sql command to drop the role. Empty if the role is a built-in role or permission isn't a database role</returns>
        public string SqlCommandToDropRole()
        {
            return Permissions.Any(x => x.OnWhat == PermissionsOnWhat.DatabaseRole)
                ? string.Format("DROP ROLE [{0}]", PrincipalName)
                : string.Empty;
        }

        /// <summary>
        /// This returns a list of GRANT/DENY SQL command to add the permissions to the role
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public IEnumerable<string> SqlCommandsToAddPermissionsToRole(DbContext db)
        {
            return Permissions.Any()
                ? Permissions.Select(x => string.Format("{0} ON [{1}]", x.SqlCommandToAddPermission(db), PrincipalName))
                : new List<string>();
        }

        public override string ToString()
        {
            return string.Join("\n", SqlCommandsToAddPermissionsToRole(null));
        }
    }
}
