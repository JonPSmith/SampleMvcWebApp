using System.Data.Entity;

namespace SampleWebApp.Identity
{
    public class IdentityDbInitializerNoCreate : IDatabaseInitializer<ApplicationDbContext>
    {
        private readonly bool _replaceAllUsers;

        public IdentityDbInitializerNoCreate(bool replaceAllUsers)
        {
            _replaceAllUsers = replaceAllUsers;
        }

        public void InitializeDatabase(ApplicationDbContext context)
        {
            InitialiseIdentityDb.InitializeAspNetUsers(context, _replaceAllUsers);
        }

    }
}