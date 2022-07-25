using System;
using System.Collections.Generic;
using NotionToUnity.Utils;

namespace NotionToUnity.DataTypes
{
    /// <summary>
    /// Helper class used to instantiate the correct local type based on the JSON.
    /// </summary>
    public static class TypeMap
    {
        private static readonly Dictionary<string, Type> STRING_TO_NOTION_TYPE = new Dictionary<string, Type>()
        {
            { "rich_text", typeof(NotionText) },
            { "title", typeof(NotionText) },
            { "select", typeof(NotionSelect) },
            { "relation", typeof(NotionRelation) }
        };

        /// <summary>
        /// Gets the local type of the corresponding name from Notion property
        /// </summary>
        /// <param name="typeString">Type string provided from Notion JSON</param>
        /// <returns>The local type</returns>
        public static Type GetType(string typeString)
        {
            if (STRING_TO_NOTION_TYPE.ContainsKey(typeString))
                return STRING_TO_NOTION_TYPE[typeString];

            Logger.LogError($"Unsupported type encountered: {typeString}");
            return null;
        }
    }
}
