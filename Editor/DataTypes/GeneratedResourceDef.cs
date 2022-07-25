using System;
#if UNITY_EDITOR
using UnityEngine;
#else
using NotionToUnity.DataTypes.NonUnity
#endif

namespace NotionToUnity.DataTypes
{
    /// <summary>
    /// Used to correlate the generated resource definition (for Unity, that's the script defining the ScriptableObject)
    /// with the database ID in Notion
    /// </summary>
    [Serializable]
    public struct GeneratedResourceDef
    {
        /// <summary>
        /// The name of the resource type (for Unity, that's the type of the ScriptableObject)
        /// </summary>
        [field: SerializeField]
        public string ResourceTypeName { get; set; }

        /// <summary>
        /// The ID of the Notion database.
        /// </summary>
        [field: SerializeField]
        public string DbId { get; set; }
    }
}
