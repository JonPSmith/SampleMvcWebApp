using System.Collections.Generic;
using System.Linq;

namespace DataLayer.Security
{
    public class SqlUserAndRoles
    {

        public string UserName { get; private set; }

        public PermissionsOnWhat UserType { get; private set; }

        public IReadOnlyCollection<SqlRole> UserRoles { get; private set; }

        public SqlUserAndRoles(string userName, PermissionsOnWhat userType, IReadOnlyCollection<SqlRole> userRoles)
        {
            UserName = userName;
            UserType = userType;
            UserRoles = userRoles;
        }

        public string SqlCommandToAddUserToLogin()
        {
            return string.Format("CREATE USER [{0}] FOR LOGIN [{0}] WITH DEFAULT_SCHEMA=[dbo]", UserName);
        }

        public string SqlCommandToRemoveUserFromLogin()
        {
            return string.Format("DROP USER [{0}]", UserName);
        }

        public IEnumerable<string> SqlCommandToRemoveUserFromItsRoles()
        {
            return UserRoles.Select(x => string.Format("ALTER [{0}] DROP MEMBER [{1}]", x.RoleName, UserName));
        }

        public IEnumerable<string> SqlCommandToAddUserToItsRoles()
        {
            return UserRoles.Select(x => string.Format("ALTER [{0}] ADD MEMBER [{1}]", x.RoleName, UserName));
        }
    }
}
