using DataLayer.DataParts;
using GenericServices;

namespace DataLayer.DataClasses
{
    public class SecureSampleWebAppDb : SampleWebAppDb, IDbContextWithValidation
    {
        public SecureSampleWebAppDb()
            : base(SqlSecurity.BuildSqlConnectionString(NameOfConnectionString)) { }
    }
}
