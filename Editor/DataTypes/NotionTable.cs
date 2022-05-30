using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine.Assertions;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace NotionToUnity.Editor
{
    public struct EnumValue
    {
        public string m_name;
        public int m_value;
        public string m_comment;
    }

    public struct EnumDefinition
    {
        public string m_name;
        public EnumValue[] m_enums;
    }

    public class NotionTable
    {
        public string Name { get; set; }

        public List<EnumDefinition> Enums { get; set; } = new List<EnumDefinition>();

        private static string GetName(JObject json)
        {
            Assert.IsNotNull(json["title"]);
            Assert.IsNotNull(json["title"][0]);
            Assert.IsNotNull(json["title"][0]["plain_text"]);
            string name = json["title"][0]["plain_text"].Value<string>();

            Assert.IsNotNull(name);
            // TODO: set some config, not just hardcoded to replace 'Database'
            return name.Replace("Database", "").Trim().RemoveSpaces();
        }

        protected static void GetPropertyNameAndType(JToken currProperty, out string name, out string type)
        {
            Assert.IsNotNull(currProperty["type"]);
            type = currProperty["type"].Value<string>();
            type = type.RemoveSpaces();

            Assert.IsNotNull(currProperty["name"]);
            name = currProperty["name"].Value<string>();
            name = name.RemoveSpaces();
        }

        protected NotionTable(JObject propertyJson, JObject contentJson)
        {
            if (propertyJson["object"].Value<string>() != "database")
            {
                Debug.LogError($"Invalid value in 'object' field: {propertyJson["object"]}");
                return;
            }

            if (contentJson["object"].Value<string>() != "list")
            {
                Debug.LogError($"Invalid value in 'object' field: {contentJson["object"]}");
                return;
            }


            Name = GetName(propertyJson);
        }

        public virtual void Resolve(Dictionary<NotionDatabaseDefinition, NotionTable> processedDatabases)
        {
            // Intentionally empty.
        }
    }
}
