using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace NotionToUnity.Editor
{
    [Serializable]
    public struct NamingConvention
    {
        [field: SerializeField]
        public string Namespace { get; set; }

        [field: SerializeField]
        public string EditorScriptPath { get; set; }

        [field: SerializeField]
        public string StructPath { get; set; }

        [field: SerializeField]
        public string SoFormat { get; set; }

        [field: SerializeField]
        public string SoPath { get; set; }

        // private string m_editorScriptPath = "Scripts/Editor/Database";
        // [SerializeField]
        // private string m_structPath = "Scripts/Database";
        // [SerializeField]
        // private string m_soFormat = "{0}Database";
    }
}
