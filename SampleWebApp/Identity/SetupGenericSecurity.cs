using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using GenericSecurity;
using GenericServices;
using SampleWebApp.Infrastructure;

namespace SampleWebApp.Identity
{
    public static class SetupGenericSecurity
    {

        public static void Setup(string unathenticatedSqlUserName, string unathenticatedSqlUserPassword)
        {
            
            //now we set up the GenericSecurity parts
            SecurityConfiguration.SetupSecurity(unathenticatedSqlUserName, unathenticatedSqlUserPassword,
                System.Configuration.ConfigurationManager.ConnectionStrings[WebUiInitialise.DatabaseConnectionStringName].ConnectionString,
                WebUiInitialise.HostType == HostTypes.Azure);

            SecurityConfiguration.GetDatabaseUser = GetDatabaseUserFromThread;
        }

        //--------------------------------------------------------------------
        //private helpers

        /// <summary>
        /// This retreaves the database user information from the current thread
        /// </summary>
        /// <returns>a DatabaseUser with the correct details in it</returns>
        private static DatabaseUser GetDatabaseUserFromThread()
        {

            if (!Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                //not logged in so use unauthenticated account
                return DatabaseUser.UnauthenticatedUser();
            }

            var identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
            if (identity == null)
                throw new InvalidOperationException("This only works with a claims based identity.");

            var userloginClaim = identity.Claims.SingleOrDefault(
                x => x.Type == InitialiseIdentityDb.DatabaseLoginClaimType);
            var passwordClaim = identity.Claims.SingleOrDefault(
                x => x.Type == InitialiseIdentityDb.DatabasePasswordClaimType);
            if (userloginClaim == null || passwordClaim == null)
                throw new InvalidOperationException(
                    "Could not find the required database information in the user's claims.");

            return new DatabaseUser(userloginClaim.Value, passwordClaim.Value);
        }

    }
}