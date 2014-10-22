#region licence
// The MIT License (MIT)
// 
// Filename: SqlCommands.cs
// Date Created: 2014/08/24
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
using System;
using System.Collections.Generic;
using System.IO;
using DataLayer.DataClasses;
using GenericSecurity.SqlInformation;
using GenericServices;
using GenericServices.Core;

namespace ServiceLayer.Security
{

    public class SqlCommands : ISqlCommands
    {

        private readonly SampleWebAppDb _db;

        public SqlCommands(SampleWebAppDb db)
        {
            _db = db;
        }

        /// <summary>
        /// This obtains the current sql security setup from the database and returns the sql commands to set it up
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetSqlCommands()
        {
            return _db.SqlCommandsCreateRolesAndPermissions();
        }

        /// <summary>
        /// This will load the appropriate sql security file and send it to the host.
        /// NOTE: This should not be used if permissions already exist
        /// </summary>
        /// <param name="appDataFilePath"></param>
        /// <param name="hostString"></param>
        /// <returns></returns>
        public ISuccessOrErrors ExecuteSqlCommandsFromFile(string appDataFilePath, string hostString)
        {
            var status = SuccessOrErrors.Success("Successfully setup database security for host {0}", hostString);

            if (string.IsNullOrEmpty(appDataFilePath))
                throw new ArgumentException("You must provide the appDataFilePath");
            if (string.IsNullOrEmpty(hostString))
                throw new ArgumentException("You must provide the hostString");

            var logger = GenericServicesConfig.GetLogger("ExecuteSqlCommandsFromFile");
            logger.InfoFormat("Called for host {0}", hostString);
            var filepath = Path.Combine(appDataFilePath, hostString + "SqlSecurity.txt");

            if (!File.Exists(filepath))
            {             
                logger.ErrorFormat("Failed to find a file at path {0}", filepath);
                return new SuccessOrErrors().AddSingleError("Could not find a file for host {0}", hostString);
            }

            var commands = File.ReadAllLines(filepath);

            var errMsg = _db.ExecuteSqlCommands(commands);

            if (errMsg == null) return status;

            //There was an error
            logger.ErrorFormat("Setup Failed: {0}", errMsg);
            status.AddSingleError("Setup Failed: {0}", errMsg);
            return status;
        }


    }
}
