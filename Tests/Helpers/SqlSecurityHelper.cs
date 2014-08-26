using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataLayer.DataClasses;
using DataLayer.Security;
using SampleWebApp.Infrastructure;

namespace Tests.Helpers
{
    static class SqlSecurityHelper
    {

        public static IEnumerable<string> SaveSqlSecuritySetup(HostTypes hostType, string loginPrefix)
        {

            var appDatafilePath = TestFileHelpers.GetSolutionDirectory() + @"\SampleWebApp\App_Data\" + FormFilename(hostType);
            IEnumerable<string> result;
            using (var db = new SampleWebAppDb())
            {
                result = db.SqlCommandsCreateUsersRolesAndPermissions(loginPrefix);
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
