using System.Collections.Generic;
using GenericServices;

namespace ServiceLayer.Security
{
    public interface ISqlCommands
    {
        /// <summary>
        /// This obtains the current sql security setup from the database and returns the sql commands to set it up
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetSqlCommands();

        /// <summary>
        /// This will load the appropriate sql security file and send it to the host.
        /// NOTE: This should not be used if permissions already exist
        /// </summary>
        /// <param name="appDataFilePath"></param>
        /// <param name="hostString"></param>
        /// <returns></returns>
        ISuccessOrErrors ExecuteSqlCommandsFromFile(string appDataFilePath, string hostString);
    }
}