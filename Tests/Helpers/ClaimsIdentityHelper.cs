#region licence
// The MIT License (MIT)
// 
// Filename: ClaimsIdentityHelper.cs
// Date Created: 2014/08/25
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
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using SampleWebApp.Identity;
using SampleWebApp.Properties;

namespace Tests.Helpers
{
    internal class ClaimsIdentityHelper
    {

        private readonly Dictionary<string, SeedUserInfo> _users;

        public ClaimsIdentityHelper()
        {
            var seedFilepath = TestFileHelpers.GetSolutionDirectory() + @"\SampleWebApp\App_Data\SeedIdentities.xml";
            var loader = new SeedUsersLoader(seedFilepath, Settings.Default.DatabaseLoginPrefix);
            var seed = loader.LoadSeedData().ToList();
            //and set the unathenticated user in SqlSecure
            SetupGenericSecurity.Setup(loader.UnauthenticatedDatabaseLogin, loader.UnauthenticatedDatabasePassword);
            seed.Add(new SeedUserInfo { Email = "bad@nospam.com", DatabaseLogin = "BadUser", DatabasePassword = "BadPassword" });
            _users = seed.ToDictionary(x => x.Email);      
        }

        public void SetUser(string emailPrefix)
        {
            var userInfo = emailPrefix == null
                ? null
                : _users[emailPrefix + "@nospam.com"];


            if (userInfo != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(InitialiseIdentityDb.DatabaseLoginClaimType, userInfo.DatabaseLogin),
                    new Claim(InitialiseIdentityDb.DatabasePasswordClaimType, userInfo.DatabasePassword)
                };
                var identity = new ClaimsIdentity(new ClaimsIdentity(CreateIdentity(userInfo)), claims);
                Thread.CurrentPrincipal = new ClaimsPrincipal(identity);
            }
            else
                Thread.CurrentPrincipal = new GenericPrincipal(CreateIdentity(null), new string[] { });

        }

        private GenericIdentity CreateIdentity(SeedUserInfo userInfo)
        {
            return new GenericIdentity(userInfo != null ? userInfo.Email : string.Empty, "unknown");
        }

    }
}
