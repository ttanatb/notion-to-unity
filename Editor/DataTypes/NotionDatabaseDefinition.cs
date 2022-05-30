using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace NotionToUnity.Editor
{
    [Serializable]
    public struct NotionDatabaseDefinition
    {
        public enum DatabaseType
        {
            Table,
            EnumDef,
        }

        [field: SerializeField]
        public string Id { get; set; }

        [field: SerializeField]
        public DatabaseType Type { get; set; }
    }
}
