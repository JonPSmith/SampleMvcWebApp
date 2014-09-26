#region licence
// The MIT License (MIT)
// 
// Filename: SqlSecurityHelper.cs
// Date Created: 2014/08/26
// 
// Copyright (c) 2014 Jon Smith (www.selectiveanalytics.com & www.thereformedprogrammer.net)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion
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
