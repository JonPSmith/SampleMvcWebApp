using System;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading;

namespace DataLayer.DataParts
{
    public class SqlSecurity
    {
        private const string DatabaseUserClaimType = "DatabaseUser";
        private const string DatabasePasswordClaimType = "DatabasePassword";

        private static string _unauthenticatedUserName = null;
        private static string _unauthenticatedUserPassword = null;
        private static bool _securityEnabled;

        private string DatabaseUserName { get; set; }
        private string DatabaseUserPassword { get; set; }

        /// <summary>
        /// This controls whether SqlSecurity is enabled or not.
        /// </summary>
        public static bool SecurityEnabled
        {
            get { return _securityEnabled; }
            set { 
                if (value && _unauthenticatedUserName == null)
                    throw new InvalidOperationException("You must call SetupUnauthenticatedUser to enable SqlSecurity.");
                _securityEnabled = value;
            }
        }

        /// <summary>
        /// This must be called to enable SqlSecurity. 
        /// </summary>
        /// <param name="userName">The database username to use for unauthenticated users</param>
        /// <param name="password">The password for the unauthenticated database users</param>
        public static void SetupUnauthenticatedDatabaseUser(string userName, string password)
        {
            _unauthenticatedUserName = userName;
            _unauthenticatedUserPassword = password;
            SecurityEnabled = true;
        }

        //private ctor because only BuildSqlConnectionString can create a SqlSecurity instance
        private SqlSecurity()
        {

            var identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
            if (identity == null)
                throw new InvalidOperationException("This only works with a claims based identity.");

            if (!identity.IsAuthenticated)
            {
                //not logged in so use unauthenticated account
                DatabaseUserName = _unauthenticatedUserName;
                DatabaseUserPassword = _unauthenticatedUserPassword;
                return;
            }

            var userNameClaim = identity.Claims.SingleOrDefault(
                x => x.Type == DatabaseUserClaimType);
            var passwordClaim = identity.Claims.SingleOrDefault(
                x => x.Type == DatabasePasswordClaimType);
            if (userNameClaim == null || passwordClaim == null)
                throw new InvalidOperationException(
                    "Could not find the required database information in the user's claims.");

            DatabaseUserName = userNameClaim.Value;
            DatabaseUserPassword = passwordClaim.Value;
        }

        /// <summary>
        /// This builds a database connection string based on the 
        /// current thread user, who must have database claims
        /// </summary>
        /// <param name="nameOfConnectionString">Should be the name of the connection string
        /// as stored in the Web/App.Config file</param>
        /// <returns>A database connection string with the database username and password in it</returns>
        internal static string BuildSqlConnectionString(string nameOfConnectionString)
        {
            var baseConnection =
                System.Configuration.ConfigurationManager.ConnectionStrings[nameOfConnectionString].ConnectionString;
            if (!SecurityEnabled)
                //In SampleMvcWebApp we default to full access in case someone is using this without security
                //In real applciations you would throw an exception
                return baseConnection;

            var dbInfo = new SqlSecurity();
            var sb = new SqlConnectionStringBuilder(baseConnection)
            {
                UserID = dbInfo.DatabaseUserName,
                Password = dbInfo.DatabaseUserPassword,
                IntegratedSecurity = false
            };
            return sb.ConnectionString;
        }
    }
}
