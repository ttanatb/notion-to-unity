using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace NotionToUnity
{
    public static class TypeMap
    {
        private static readonly Dictionary<string, Type> STRING_TO_NOTION_TYPE = new Dictionary<string, Type>()
        {
            { "rich_text", typeof(NotionText) },
            { "title", typeof(NotionText) },
            { "select", typeof(NotionSelect) },
        };

        public static Type GetType(string typeString)
        {
            return STRING_TO_NOTION_TYPE[typeString];
        }
    }
}
