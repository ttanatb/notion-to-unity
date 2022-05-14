using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine.Assertions;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace NotionToUnity
{
    public struct NotionEnumValue
    {
        public string m_name;
        public int m_value;
    }

    public struct NotionEnumDefinition
    {
        public string m_name;
        public NotionEnumValue[] m_enums;
    }

    public struct NotionProperty
    {
        public string m_name;
        public string m_type;
    }

    public class DatabaseProperty
    {
        public string Name { get; set; }

        public List<NotionEnumDefinition> Enums { get; set; } = new List<NotionEnumDefinition>();

        public List<NotionProperty> Fields { get; set; } = new List<NotionProperty>();

        public DatabaseProperty(JObject json)
        {
            if (json["object"].Value<string>() != "database")
            {
                Debug.LogError($"Invalid value in 'object' field: {json["object"]}");
                return;
            }

            Assert.IsNotNull(json["title"]);
            Assert.IsNotNull(json["title"][0]);
            Assert.IsNotNull(json["title"][0]["plain_text"]);
            Name = json["title"][0]["plain_text"].Value<string>();

            Assert.IsNotNull(Name);
            Name = Name.Replace("Database", "").Trim();

            var hasher = MD5.Create();

            foreach (var property in json["properties"])
            {
                // Not sure why, but need to access child here.
                var currProperty = property.First;
                Assert.IsNotNull(currProperty);

                Assert.IsNotNull(currProperty["type"]);
                string type = currProperty["type"].Value<string>();
                type = type.RemoveSpaces();

                Assert.IsNotNull(currProperty["name"]);
                string name = currProperty["name"].Value<string>();
                name = name.RemoveSpaces();

                if (type == "select")
                {
                    // Create enum values
                    Assert.IsNotNull(currProperty["select"]);
                    var opts = currProperty["select"]["options"];

                    Assert.IsNotNull(opts);
                    var enums = new List<NotionEnumValue>();

                    foreach (var opt in opts)
                    {
                        Assert.IsNotNull(opt["name"]);
                        string optName = opt["name"].Value<string>();

                        Assert.IsNotNull(opt["id"]);
                        string id = opt["id"].Value<string>();
                        Assert.IsNotNull(id);

                        int value = BitConverter.ToInt32(
                            hasher.ComputeHash(Encoding.UTF8.GetBytes(id)), 0);

                        enums.Add(new NotionEnumValue(){ m_name = optName.RemoveSpaces(), m_value = value});
                    }

                    Enums.Add(new NotionEnumDefinition()
                    {
                        m_name = $"{name}Enum",
                        m_enums = enums.ToArray(),
                    });

                    Fields.Add(new NotionProperty()
                    {
                        m_name = name,
                        m_type = $"{name}Enum"
                    });
                }
                else if (type == "title" || type == "rich_text")
                {
                    Fields.Add(new NotionProperty()
                    {
                        m_name = name,
                        m_type = "string"
                    });
                }
            }
        }
    }
}
