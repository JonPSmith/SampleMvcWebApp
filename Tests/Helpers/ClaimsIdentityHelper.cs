using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using DataLayer.Security;
using SampleWebApp.Identity;

namespace Tests.Helpers
{
    internal class ClaimsIdentityHelper
    {

        private Dictionary<string, SeedUserInfo> _users;

        public ClaimsIdentityHelper()
        {
            var seedFilepath = TestFileHelpers.GetSolutionDirectory() + @"\SampleWebApp\App_Data\SeedIdentities.xml";
            var loader = new SeedUsersLoader(seedFilepath);
            loader.DataFileExists.ShouldEqual(true);
            var seed = loader.LoadSeedData().ToList();
            seed.Add(new SeedUserInfo { Email = "bad@nospam.com", DatabaseLogin = "BadUser", DatabasePassword = "BadPassword" });
            _users = seed.ToDictionary(x => x.Email);      //this also sets unathenticated user in SqlSecure
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
                    new Claim(SqlSecure.DatabaseLoginClaimType, userInfo.DatabaseLogin),
                    new Claim(SqlSecure.DatabasePasswordClaimType, userInfo.DatabasePassword)
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
