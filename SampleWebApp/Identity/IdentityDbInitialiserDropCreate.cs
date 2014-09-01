using System.Data.Entity;

namespace SampleWebApp.Identity
{
    public class IdentityDbInitialiserDropCreate : DropCreateDatabaseAlways<ApplicationDbContext>
    {

        private readonly bool _replaceAllUsers;

        public IdentityDbInitialiserDropCreate(bool replaceAllUsers)
        {
            _replaceAllUsers = replaceAllUsers;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            InitialiseIdentityDb.InitializeAspNetUsers(context, _replaceAllUsers);
            base.Seed(context);
        }
    }
}