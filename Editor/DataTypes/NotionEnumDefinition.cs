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
    public class NotionEnumDefinition : NotionTable
    {
        public NotionEnumDefinition(JObject propertyJson, JObject contentJson) : base(propertyJson, contentJson)
        {
            // Check expected columns
            bool hasTitle = false;
            bool hasComment = false;
            Assert.IsNotNull(propertyJson["properties"]);
            foreach (var property in propertyJson["properties"])
            {
                // Not sure why, but need to access child here.
                var currProperty = property.First;
                Assert.IsNotNull(currProperty);

                GetPropertyNameAndType(currProperty, out string name, out string type);

                if (type == "title")
                    hasTitle = true;
                else if (type == "rich_text" && name.ToLower() == "comment")
                    hasComment = true;
            }

            if (!hasTitle) Debug.LogError($"Enum Definition Database ({Name}) is missing required title field.");
            if (!hasComment) Debug.Log($"Enum Definition Database ({Name}) is missing optional comment field.");

            // Hasher? I barely even know 'er!
            var hasher = MD5.Create();
            var enumValues = new List<EnumValue>();

            Assert.IsNotNull(contentJson["results"]);
            foreach (var row in contentJson["results"])
            {
                var properties = row["properties"];
                Assert.IsNotNull(properties);
                Assert.IsNotNull(properties["Name"]);
                var name = new NotionText(properties["Name"], null);
                if (string.IsNullOrEmpty(name.Value))
                    continue;

                var comment = new NotionText(properties["Comment"], null);

                Assert.IsNotNull(row["id"]);
                string id = row["id"].Value<string>();

                Assert.IsNotNull(id);
                int value = BitConverter.ToInt32(
                    hasher.ComputeHash(Encoding.UTF8.GetBytes(id)), 0);

                enumValues.Add(new EnumValue()
                {
                    m_comment = comment.Value,
                    m_name =  name.Value,
                    m_value =  value,
                });
            }

            Enums.Add(new EnumDefinition()
            {
                m_enums =  enumValues.ToArray(),
                m_name = Name,
            });

            hasher.Dispose();
        }
    }
}
