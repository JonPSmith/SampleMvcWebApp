using DataLayer.Security;
using GenericServices;

namespace DataLayer.DataClasses
{
    public class SecureSampleWebAppDb : SampleWebAppDb, IDbContextWithValidation
    {
        public SecureSampleWebAppDb()
            : base(SqlSecure.BuildSqlConnectionString(NameOfConnectionString, EfConfiguration.IsAzure)) { }
    }
}
