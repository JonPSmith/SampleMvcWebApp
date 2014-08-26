using System.Collections.Generic;

namespace ServiceLayer.Security
{
    public interface ISqlCommands
    {
        /// <summary>
        /// This obtains the current sql security setup from the database and returns the sql commands to set it up
        /// </summary>
        /// <param name="loginPrefix"></param>
        /// <returns></returns>
        IEnumerable<string> GetSqlCommands(string loginPrefix = null);
    }
}