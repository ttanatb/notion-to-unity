using System;
#if UNITY_EDITOR
using UnityEngine;
#else
using NotionToUnity.DataTypes.NonUnity
#endif

namespace NotionToUnity.JsonProcessor.Types
{
    /// <summary>
    /// Used to configure & format the generated code.
    /// </summary>
    [Serializable]
    public struct NamingConvention
    {
        /// <summary>
        /// Namespace of the generated script.
        /// </summary>
        [field: SerializeField]
        public string Namespace { get; set; }

        /// <summary>
        /// File path for generated files that's meant for the Editor.
        /// </summary>
        [field: SerializeField]
        public string EditorScriptPath { get; set; }

        /// <summary>
        /// File path for local structs for the local database.
        /// </summary>
        [field: SerializeField]
        public string StructPath { get; set; }

        /// <summary>
        /// Path to place the generated resource object (for Unity, it's where the
        /// generated ScriptableObject assets go).
        /// </summary>
        [field: SerializeField]
        public string ResourceFilePath { get; set; }
        
        /// <summary>
        /// The format for naming resource files (for Unity, it's how the the ScriptableObject files are named).
        /// </summary>
        [field: SerializeField]
        public string ResourceClassNameFormat { get; set; }

        /// <summary>
        /// The number of indents for each nested bracket.
        /// </summary>
        [field: SerializeField]
        public int IndentCount { get; set; }
        
        /// <summary>
        /// The character used for indentation.
        /// </summary>
        [field: SerializeField]
        public IndentConfig.IndentType IndentType { get; set; }
    }
}
