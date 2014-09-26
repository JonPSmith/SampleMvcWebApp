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

            var logger = ServicesConfiguration.GetLogger("ExecuteSqlCommandsFromFile");
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
