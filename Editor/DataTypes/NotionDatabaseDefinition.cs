using System;
#if UNITY_EDITOR
using UnityEngine;
#else
using NotionToUnity.DataTypes.NonUnity
#endif

namespace NotionToUnity.DataTypes
{
    /// <summary>
    /// Local definition of a Notion database object.
    /// </summary>
    [Serializable]
    public struct NotionDatabaseDefinition
    {
        /// <summary>
        /// The 'types' of Notion databases we care about.
        /// </summary>
        public enum DatabaseType
        {
            /// <summary>
            /// This is a generic database used to keep track of data in the game like an item database.
            /// </summary>
            Table,
            
            /// <summary>
            /// This is also technically a table in Notion, but it only contains a list of enum definitions,
            /// mainly used to define event flags and what not.
            /// </summary>
            EnumDef,
        }

        /// <summary>
        /// ID of the database in Notion.
        /// </summary>
        [field: SerializeField]
        public string Id { get; set; }

        /// <summary>
        /// The type of this database.
        /// </summary>
        [field: SerializeField]
        public DatabaseType Type { get; set; }
    }
}
