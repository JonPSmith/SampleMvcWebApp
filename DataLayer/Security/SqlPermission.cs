using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using DataLayer.Security.Internal;

[assembly: InternalsVisibleTo("Tests")]

namespace DataLayer.Security
{
    public enum PermissionsOnWhat { NotHandled, SqlUser, WindowsUser, WindowsGroup, ApplicationRole, DatabaseRole}
    public enum PermissionStates { NotHandled, Deny, Revoke, Grant }
    [Flags]
    public enum PermissionTypeFlags { NotHandled = 0, Insert = 1, Select = 2, Update = 4, Delete = 8, References = 16}

    public class SqlPermission
    {
        public const char SqlUserChar = 'S';
        public const char WindowsUserChar = 'U';
        public const char WindowsGroupChar = 'G';

        private static readonly Dictionary<char, PermissionsOnWhat> PermissionOnWhatLookup = new Dictionary<char, PermissionsOnWhat>
        {
            {SqlUserChar, PermissionsOnWhat.SqlUser},
            {WindowsUserChar, PermissionsOnWhat.WindowsUser},
            {WindowsGroupChar, PermissionsOnWhat.WindowsGroup},
            {'A', PermissionsOnWhat.ApplicationRole},
            {'R', PermissionsOnWhat.DatabaseRole},
        };

        private static readonly Dictionary<char, PermissionStates> PermissionStateLookup = new Dictionary<char, PermissionStates>
        {
            {'D', PermissionStates.Deny},
            {'R', PermissionStates.Revoke},
            {'G', PermissionStates.Grant},
            {'W', PermissionStates.Grant},
        };

        private static readonly Dictionary<string, PermissionTypeFlags> PermissionTypeLookup = new Dictionary<string, PermissionTypeFlags>
        {
            {"IN", PermissionTypeFlags.Insert},
            {"SL", PermissionTypeFlags.Select},
            {"UP", PermissionTypeFlags.Update},
            {"DL", PermissionTypeFlags.Delete},
            {"RF", PermissionTypeFlags.References}
        };

        /// <summary>
        /// This holds what the permission is on, e.g. a sql user or a database role
        /// </summary>
        public PermissionsOnWhat OnWhat { get; private set; }

        /// <summary>
        /// This says what sort of permission is being done, i.e. Grant, Deny, Revoke
        /// </summary>
        public PermissionStates State { get; private set; }

        /// <summary>
        /// A set of flags that say what is being set to the state
        /// </summary>
        public PermissionTypeFlags Flags { get; private set; }

        /// <summary>
        /// What schema is this permission linked to
        /// </summary>
        public string SchemaName { get; private set; }

        /// <summary>
        /// Name of the object this permission is applied to
        /// </summary>
        public string ObjectName { get; private set; }

        /// <summary>
        /// If this is not 0 then it is the column index (starting from 1 for first column)
        /// </summary>
        public int ColumnId { get; private set; }


        public override string ToString()
        {
            return string.Format("{0}: {1}", OnWhat, SqlCommandToAddPermission(null));
        }

        //--------------------------------------------
        //internal parts

        internal SqlPermission(SqlRoleRow readRow)
            : this(ConvertDataBasePrincipalType(readRow.DatabasePrincipalType), 
                   ConvertDatabasePermissionState(readRow.DatabasePermissionState), 
                   ConvertDatabasePermissionType(readRow.DatabasePermissionType),
                   readRow.SchemasName, readRow.ObjectName, readRow.MinorId) { }

        internal SqlPermission(PermissionsOnWhat onWhat, PermissionStates state, PermissionTypeFlags flags, 
            string schemaName, string objectName, int columnId)
        {
            OnWhat = onWhat;
            State = state;
            Flags = flags;
            SchemaName = schemaName;
            ObjectName = objectName;
            ColumnId = columnId;
        }

        /// <summary>
        /// This tries to combine the next SqlPermission with the last one. 
        /// If they can be combined then it returns true
        /// </summary>
        /// <param name="last"></param>
        /// <returns>true if next has been combined into last. False otherwise</returns>
        internal bool TryCombine( SqlPermission last)
        {
            if (last == null) return false;
            if (last.OnWhat != OnWhat || last.State != State || last.SchemaName != SchemaName ||
                last.ObjectName != ObjectName || last.ColumnId != ColumnId) return false;

            last.Flags |= Flags;
            return true;
        }

        internal string SqlCommandToAddPermission(DbContext db)
        {
            if (db == null)
                //can't do proper lookup so do incorrect, but viewable text, so ToString works
                return
                    SqlPermissionCommandWithColumnText(ColumnId == 0
                        ? string.Empty
                        : string.Format("(Column[{0}])", ColumnId));

            //Otherwise need to look up column (if present) and insert it
            var columnText = GetColumnNameInBrackets(db);
            return SqlPermissionCommandWithColumnText(columnText);
        }

        //------------------------------------------------------
        //internal/private helpers

        private string GetColumnNameInBrackets(DbContext db)
        {
            if (ColumnId == 0) return string.Empty;

            var columnText = string.Format("({0})", db.Database.SqlQuery<string>(
                string.Format( 
                    "SELECT COLUMN_NAME FROM information_schema.columns WHERE TABLE_NAME = '{0}' AND ORDINAL_POSITION = {1}",
                    ObjectName, ColumnId)).First());
            return columnText;
        }

        private string SqlPermissionCommandWithColumnText( string columnText)
        {
            return string.Format("{0} {1} ON OBJECT::{2}.{3}{4}", State.ToString().ToUpperInvariant(),
                Flags.ToString().ToUpperInvariant(), SchemaName, ObjectName, columnText);
        }

        internal static bool IsUser(string typeString)
        {
            return typeString[0] == SqlUserChar || typeString[0] == WindowsUserChar || typeString[0] == WindowsGroupChar;
        }

        internal static PermissionsOnWhat ConvertDataBasePrincipalType(string typeString)
        {
            PermissionsOnWhat onWhat;
            PermissionOnWhatLookup.TryGetValue(typeString[0], out onWhat);
            return onWhat;
        }

        private static PermissionStates ConvertDatabasePermissionState(string stateString)
        {
            PermissionStates state;
            PermissionStateLookup.TryGetValue(stateString[0], out state);
            return state;
        }

        private static PermissionTypeFlags ConvertDatabasePermissionType(string typeString)
        {
            PermissionTypeFlags flags;
            PermissionTypeLookup.TryGetValue(typeString.TrimEnd(), out flags);
            return flags;
        }
    }
}
