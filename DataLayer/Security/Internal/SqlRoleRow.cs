using System.Collections.Generic;
using System.Data.Entity;

namespace DataLayer.Security.Internal
{
    internal class SqlRoleRow
    {

        public string DatabasePrincipalsName { get; set; }

        public string DatabasePrincipalType { get; set; }

        public string DatabasePermissionState { get; set; }

        public string DatabasePermissionType { get; set; }

        public string SchemasName { get; set; }

        public string ObjectName { get; set; }

        public int MinorId { get; set; }

        /// <summary>
        /// This returns all the Roles in the database.
        /// Note: it sorts in the order of the PermissionName, schemeName, ObjectName and ColumnId.
        /// That sorting is needed for TryCombine to combine similar permissions
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static IEnumerable<SqlRoleRow> ReadRolesRows(DbContext db)
        {
            return db.Database.SqlQuery<SqlRoleRow>(@"SELECT pr.name as DatabasePrincipalsName, pr.type AS DatabasePrincipalType, 
pe.state AS DatabasePermissionState, pe.type AS DatabasePermissionType, s.name AS SchemasName, o.name AS ObjectName, pe.minor_id As MinorId
FROM sys.database_principals AS pr
JOIN sys.database_permissions AS pe
    ON pe.grantee_principal_id = pr.principal_id
JOIN sys.objects AS o
    ON pe.major_id = o.object_id
JOIN sys.schemas AS s
    ON o.schema_id = s.schema_id
ORDER BY pr.name, s.name, o.name, pe.minor_id");
        }


        public override string ToString()
        {
            return string.Format("DataBasePrincipalsName: {0}, DatabasePrincipalType: {1}, DatabasePermissionState: {2}, DatabasePermissionType: {3}, SchemasName: {4}, ObjectName: {5}, MinorId: {6}", 
                DatabasePrincipalsName, DatabasePrincipalType, DatabasePermissionState, DatabasePermissionType, SchemasName, ObjectName, MinorId);
        }
    }
}
