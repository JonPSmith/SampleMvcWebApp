using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.DataClasses
{
    public class EfConfiguration : DbConfiguration
    {
        /// <summary>
        /// This flag should be set to true if we are working with an Azure database.
        /// It should be set before EF uses the configuration, i.e. beofre the first access 
        /// </summary>
        public static bool IsAzure { get; internal set; }

        public EfConfiguration()
        {
            if (IsAzure)
                SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
        }

    }
}
