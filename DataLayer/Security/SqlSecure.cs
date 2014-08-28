using System;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading;

namespace DataLayer.Security
{
    public class SqlSecure
    {
        public const string DatabaseLoginClaimType = "DatabaseLogin";
        public const string DatabasePasswordClaimType = "DatabasePassword";

        private static string _unauthenticatedUserName = null;
        private static string _unauthenticatedUserPassword = null;
        private static bool _securityEnabled;

        private string DatabaseLoginName { get; set; }
        private string DatabaseUserPassword { get; set; }

        /// <summary>
        /// This controls whether SqlSecurity is enabled or not.
        /// </summary>
        public static bool SecurityEnabled
        {
            get { return _securityEnabled; }
            set { 
                if (value && _unauthenticatedUserName == null)
                    throw new InvalidOperationException("You must call SetupUnauthenticatedUser to enable SqlSecure.");
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
        private SqlSecure()
        {

            if (!Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                //not logged in so use unauthenticated account
                DatabaseLoginName = _unauthenticatedUserName;
                DatabaseUserPassword = _unauthenticatedUserPassword;
                return;
            }

            var identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
            if (identity == null)
                throw new InvalidOperationException("This only works with a claims based identity.");

            var userloginClaim = identity.Claims.SingleOrDefault(
                x => x.Type == DatabaseLoginClaimType);
            var passwordClaim = identity.Claims.SingleOrDefault(
                x => x.Type == DatabasePasswordClaimType);
            if (userloginClaim == null || passwordClaim == null)
                throw new InvalidOperationException(
                    "Could not find the required database information in the user's claims.");

            DatabaseLoginName = userloginClaim.Value;
            DatabaseUserPassword = passwordClaim.Value;
        }

        /// <summary>
        /// This builds a database connection string based on the 
        /// current thread user, who must have database claims
        /// </summary>
        /// <param name="nameOfConnectionString">Should be the name of the connection string
        /// as stored in the Web/App.Config file</param>
        /// <param name="isAzure">true if azure database</param>
        /// <returns>A database connection string with the database username and password in it</returns>
        internal static string BuildSqlConnectionString(string nameOfConnectionString, bool isAzure)
        {
            var baseConnection =
                System.Configuration.ConfigurationManager.ConnectionStrings[nameOfConnectionString].ConnectionString;
            if (!SecurityEnabled)
                //In SampleMvcWebApp we default to full access in case someone is using this without security
                //In real applications you would throw an exception
                return baseConnection;

            var dbInfo = new SqlSecure();
            var sb = new SqlConnectionStringBuilder(baseConnection)
            {
                Password = dbInfo.DatabaseUserPassword,
                IntegratedSecurity = false
            };
            sb.UserID = isAzure
                ? FormAzureUserId(dbInfo.DatabaseLoginName, sb.DataSource)
                : dbInfo.DatabaseLoginName;

            return sb.ConnectionString;
        }

        /// <summary>
        /// Azure UserId needs to be in the format |LoginName|@|DatabaseName|
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        private static string FormAzureUserId(string loginName, string dataSource)
        {
            var indexOfDot = dataSource.IndexOf('.');
            if (indexOfDot < 4)
                throw new InvalidOperationException("The format of the datasource does not fit the azure format.");
            var databaseName = dataSource.Substring(4, indexOfDot - 4);
            return string.Format("{0}@{1}", loginName, databaseName);
        }
    }
}
