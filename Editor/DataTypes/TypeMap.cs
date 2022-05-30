using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace NotionToUnity.Editor
{
    public static class TypeMap
    {
        private static readonly Dictionary<string, Type> STRING_TO_NOTION_TYPE = new Dictionary<string, Type>()
        {
            { "rich_text", typeof(NotionText) },
            { "title", typeof(NotionText) },
            { "select", typeof(NotionSelect) },
            { "relation", typeof(NotionRelation) }
        };

        public static Type GetType(string typeString)
        {
            if (STRING_TO_NOTION_TYPE.ContainsKey(typeString))
                return STRING_TO_NOTION_TYPE[typeString];

            Debug.LogError($"Unsupported type encountered: {typeString}");
            return null;
        }
    }
}
