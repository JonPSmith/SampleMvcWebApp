using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataLayer.DataClasses;
using DataLayer.Security;
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
        /// <param name="loginPrefix"></param>
        /// <returns></returns>
        public IEnumerable<string> GetSqlCommands(string loginPrefix = null)
        {
            return _db.SqlCommandsCreateUsersRolesAndPermissions(loginPrefix);
        }

        //public IEnumerable<string> LoadSqlCommandsFromFile(SqlCommandDto dto)
        //{
        //    if (string.IsNullOrEmpty(dto.AppDataFilePath))
        //        throw new ArgumentException("You must provide the AppDataFilePath");
        //    if (string.IsNullOrEmpty(dto.Filename))
        //        throw new ArgumentException("You must provide the Filename");

        //    var filepath = Path.Combine(dto.AppDataFilePath, dto.Filename + ".txt");
        //    return File.ReadAllLines(filepath);
        //}

        //public ISuccessOrErrors ExecuteSqlCommandsFromFile(SqlCommandDto dto)
        //{
        //    if (string.IsNullOrEmpty(dto.AppDataFilePath))
        //        throw new ArgumentException("You must provide the AppDataFilePath");
        //    if (string.IsNullOrEmpty(dto.Filename))
        //        throw new ArgumentException("You must provide the Filename");

        //    var filepath = Path.Combine(dto.AppDataFilePath, dto.Filename + ".txt");
        //    var commands = File.ReadAllLines(filepath);

        //    var errMsg = _db.ExecuteSqlCommands(commands);

        //    var status = SuccessOrErrors.Success("Successfully setup database security from file {0}", dto.Filename + ".txt");
        //    if (errMsg != null)
        //        status.AddSingleError("Setup Failed: {0}", errMsg);

        //    return status;
        //}


    }
}
