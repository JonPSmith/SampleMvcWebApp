using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Web;
using DataLayer.Security;
using GenericServices;
using GenericServices.Logger;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SampleWebApp.Properties;

namespace SampleWebApp.Identity
{
    public static class InitialiseIdentityDb
    {

        private static IGenericLogger _logger;

        public static void Initialise(bool resetIdentityDbContent, bool canCreateDatabase)
        {

            _logger = GenericLoggerFactory.GetLogger("InitialiseIdentityDb");
            _logger.InfoFormat("Initialising with resetIdentityDbContent = {0} and canCreateDatabase = {1}", resetIdentityDbContent, canCreateDatabase);

            //Initialiser for the database.
            if (canCreateDatabase)
                //This initialiser will DropCreate the database every time (it has to so the seed is run)
                Database.SetInitializer(new IdentityDbInitialiserDropCreate(true));     //NOTE: we use dropcreate so have to always run
            else
                //This initializer will not try to change the database, but will run the initialize, which calls InitializeAspNetUsers
                Database.SetInitializer(new IdentityDbInitializerNoCreate(resetIdentityDbContent));
        }

        /// <summary>
        /// This will 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="resetIdentityDbContent">If true it will setup/reset all the users in the seed file</param>
        internal static void InitializeAspNetUsers(ApplicationDbContext context, bool resetIdentityDbContent)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            //var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            var loader = new SeedUsersLoader(HttpContext.Current.Server.MapPath("~/App_Data/SeedIdentities.xml"), Settings.Default.DatabaseLoginPrefix);
            if (!loader.DataFileExists)
                //We don't fail if there isnt a file because this is a open-source project
                //and people may run the application without users
                return;

            //always setup unauthenticaed user as not stored in Identity (this also turns on SqlSecure)
            SqlSecure.SetupUnauthenticatedDatabaseUser(loader.UnauthenticatedDatabaseLogin, loader.UnauthenticatedDatabasePassword);

            if (!resetIdentityDbContent) return;
            _logger.InfoFormat("Full reset of users asked for");

            foreach (var seedUser in loader.LoadSeedData())
            {
                var foundUser = userManager.FindByEmail(seedUser.Email);
                if (foundUser != null)
                {
                    //We are replacing all existing user data
                    userManager.Delete(foundUser);
                    _logger.InfoFormat("Deleting user {0} prior to replacement.", foundUser.DisplayName);
                }

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

                AddClaimAndCheck(userManager, user, SqlSecure.DatabaseLoginClaimType, seedUser.DatabaseLogin);
                AddClaimAndCheck(userManager, user, SqlSecure.DatabasePasswordClaimType, seedUser.DatabasePassword);
                _logger.InfoFormat("Successfull created user {0}.", user.DisplayName);
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