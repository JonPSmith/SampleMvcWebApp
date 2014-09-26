using GenericSecurity;
using GenericServices;

namespace DataLayer.DataClasses
{
    public class SecureSampleWebAppDb : SampleWebAppDb, IGenericServicesDbContext
    {
        public SecureSampleWebAppDb()
            : base(DatabaseUser.BuildConnectionString()) { }
    }
}
