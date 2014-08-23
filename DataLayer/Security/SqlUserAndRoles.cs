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
        /// <summary>
        /// This created the SQL command to create user and connect it to a login name
        /// </summary>
        /// <param name="loginPrefix">The login name is created by prepending this parameter to the database username</param>
        /// <returns></returns>
        public string SqlCommandToAddUserToLogin(string loginPrefix)
        {
            
            return string.Format("CREATE USER [{0}] FOR LOGIN [{1}{0}] WITH DEFAULT_SCHEMA=[dbo]", 
                UserName, loginPrefix ?? string.Empty);
        }

        public string SqlCommandToRemoveUserFromLogin()
        {
            return string.Format("DROP USER [{0}]", UserName);
        }

        public IEnumerable<string> SqlCommandToRemoveUserFromItsRoles()
        {
            return UserRoles.Select(x => string.Format("ALTER [{0}] DROP MEMBER [{1}]", x.PrincipalName, UserName));
        }

        public IEnumerable<string> SqlCommandToAddUserToItsRoles()
        {
            return UserRoles.Select(x => string.Format("ALTER [{0}] ADD MEMBER [{1}]", x.PrincipalName, UserName));
        }
    }
}
