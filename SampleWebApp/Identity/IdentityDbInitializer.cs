using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SampleWebApp.Identity
{
    public class IdentityDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        private const string DatabaseUserClaimType = "DatabaseUser";
        private const string DatabasePasswordClaimType = "DatabasePassword";

        private readonly bool _replaceAllUsers;

        public IdentityDbInitializer(bool replaceAllUsers)
        {
            _replaceAllUsers = replaceAllUsers;
        }


        //Remember this runs EVERY time the initialiser is called, which is what we need
        protected override void Seed(ApplicationDbContext context)
        {
            InitializeAspNetUsers(context);
            base.Seed(context);
        }

        private void InitializeAspNetUsers(ApplicationDbContext context)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            //var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            var loader = new SeedUsersLoader(HttpContext.Current.Server.MapPath("~/App_Data/SeedIdentities.xml"));
            if (!loader.DataFileExists) 
                //We don't fail if there isnt a file because this is a open-source project
                //and people may run the application without users
                return;

            foreach (var seedUser in loader.LoadSeedData())
            {
                var foundUser = userManager.FindByEmail(seedUser.Email);
                if (foundUser != null && !_replaceAllUsers) continue;

                if (_replaceAllUsers && foundUser != null)
                    //We are replacing all existing user data
                    userManager.Delete(foundUser);

                var user = new ApplicationUser
                {
                    DisplayName = seedUser.DisplayName,
                    OriginalPassword = seedUser.OriginalPassword,
                    UserName = seedUser.Email,
                    Email = seedUser.Email
                };
                var result = userManager.Create(user, seedUser.OriginalPassword);
                if (!result.Succeeded)
                    throw new InvalidOperationException(string.Format("Could not create user for {0}. Errors = {1}",
                        user.UserName, string.Join(", ", result.Errors)));

                AddClaimAndCheck(userManager, user, DatabaseUserClaimType, seedUser.DatabaseUser);
                AddClaimAndCheck(userManager, user, DatabasePasswordClaimType, seedUser.DatabasePassword);
            }
        }

        private static void AddClaimAndCheck(UserManager<ApplicationUser, string> userManager, IUser<string> user, 
            string claimName, string claimValue)
        {
            var result = userManager.AddClaim(user.Id, new Claim(claimName, claimValue));
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Format("Could not add claim {0} to user {1}. Errors = {2}",
                    user.UserName, claimName, string.Join(", ", result.Errors)));
        }


    }
}