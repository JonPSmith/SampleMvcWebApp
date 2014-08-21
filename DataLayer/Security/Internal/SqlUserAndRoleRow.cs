using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DataLayer.Security.Internal
{
    internal class SqlUserAndRoleRow
    {
        public string UserName { get; set; }
        public string UserType { get; set; }
        public string RoleName { get; set; }


        public static IEnumerable<SqlUserAndRoleRow> GetUsersAndRoleStrings(DbContext db, bool sqlUser)
        {
            
            var allUsers = db.Database.SqlQuery<SqlUserAndRoleRow>(
                @"select mp.name as UserName, rp.name as RoleName, mp.type as UserType
from sys.database_role_members drm
join sys.database_principals rp on (drm.role_principal_id = rp.principal_id)
join sys.database_principals mp on (drm.member_principal_id = mp.principal_id)
ORDER BY UserName");

            if (sqlUser)
                return allUsers.Where(x => x.UserType[0] == SqlPermission.SqlUserChar);
            else
                return allUsers.Where(x => x.UserType[0] == SqlPermission.WindowsUserChar || x.UserType[0] == SqlPermission.WindowsGroupChar);
        }
    }
}
