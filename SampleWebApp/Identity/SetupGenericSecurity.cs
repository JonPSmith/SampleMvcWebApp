#region licence
// The MIT License (MIT)
// 
// Filename: SetupGenericSecurity.cs
// Date Created: 2014/09/26
// 
// Copyright (c) 2014 Jon Smith (www.selectiveanalytics.com & www.thereformedprogrammer.net)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion
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
            GenericSecurityConfig.SetupSecurity(unathenticatedSqlUserName, unathenticatedSqlUserPassword,
                WebUiInitialise.GetDbConnectionString(),
                WebUiInitialise.HostType == HostTypes.Azure);

            GenericSecurityConfig.GetDatabaseUser = GetDatabaseUserFromThread;
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