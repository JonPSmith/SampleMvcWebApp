using System.Collections.Generic;
using System.IO;
using DataLayer.DataClasses;
using GenericSecurity.SqlInformation;
using SampleWebApp.Infrastructure;

namespace Tests.Helpers
{
    static class SqlSecurityHelper
    {

        public static IEnumerable<string> SaveSqlSecuritySetup(HostTypes hostType)
        {

            var appDatafilePath = TestFileHelpers.GetSolutionDirectory() + @"\SampleWebApp\App_Data\" + FormFilename(hostType);
            IEnumerable<string> result;
            using (var db = new SampleWebAppDb())
            {
                result = db.SqlCommandsCreateRolesAndPermissions();
                File.WriteAllLines(appDatafilePath, result);          //writes to app data
            }
            return result;
        }

        private static string FormFilename(HostTypes hostType)
        {
            return hostType.ToString() + "SqlSecurity.txt";
        }
    }
}
