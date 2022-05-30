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
    public struct FieldDefinition
    {
        public string m_name;
        public string m_type;
    }

    public struct UnresolvedFieldDefinition
    {
        public string m_name;
        public string m_dbId;
    }

    public class NotionDatabase : NotionTable
    {
        public List<FieldDefinition> Fields { get; set; } = new List<FieldDefinition>();

        public List<UnresolvedFieldDefinition> UnresolvedFields { get; set; } = new List<UnresolvedFieldDefinition>();

        public NotionDatabase(JObject propertyJson, JObject contentJson) : base(propertyJson, contentJson)
        {
            // Hasher? I barely even know 'er!
            var hasher = MD5.Create();

            Assert.IsNotNull(propertyJson["properties"]);
            foreach (var property in propertyJson["properties"])
            {
                // Not sure why, but need to access child here.
                var currProperty = property.First;
                Assert.IsNotNull(currProperty);

                GetPropertyNameAndType(currProperty, out string name, out string type);

                if (type == "select")
                {
                    // Create enum values
                    Assert.IsNotNull(currProperty["select"]);
                    var opts = currProperty["select"]["options"];

                    Assert.IsNotNull(opts);
                    var enums = new List<EnumValue>();

                    foreach (var opt in opts)
                    {
                        Assert.IsNotNull(opt["name"]);
                        string optName = opt["name"].Value<string>();

                        Assert.IsNotNull(opt["id"]);
                        string id = opt["id"].Value<string>();
                        Assert.IsNotNull(id);

                        int value = BitConverter.ToInt32(
                            hasher.ComputeHash(Encoding.UTF8.GetBytes(id)), 0);

                        enums.Add(new EnumValue() { m_name = optName.RemoveSpaces(), m_value = value });
                    }

                    Enums.Add(new EnumDefinition()
                    {
                        m_name = $"{name}Enum",
                        m_enums = enums.ToArray(),
                    });

                    Fields.Add(new FieldDefinition()
                    {
                        m_name = name,
                        m_type = $"{name}Enum"
                    });
                }
                else if (type == "title" || type == "rich_text")
                {
                    Fields.Add(new FieldDefinition()
                    {
                        m_name = name,
                        m_type = "string"
                    });
                }
                else if (type == "relation")
                {
                    Assert.IsNotNull(currProperty[type]);
                    Assert.IsNotNull(currProperty[type]["database_id"]);

                    string dbId = currProperty[type]["database_id"].Value<string>();
                    Assert.IsNotNull(dbId);
                    UnresolvedFields.Add(new UnresolvedFieldDefinition()
                    {
                        m_name = name,
                        m_dbId = dbId.Replace("-", ""),
                    });
                }
                else
                {
                    Debug.LogWarning($"Encountered unsupported property type: {type}");
                }
            }

            hasher.Dispose();
        }

        public override void Resolve(Dictionary<NotionDatabaseDefinition, NotionTable> processedDatabases)
        {
            foreach (var field in UnresolvedFields)
            {
                // var res = default(KeyValuePair<NotionDatabaseDefinition, NotionDatabase>);
                var res = processedDatabases.FirstOrDefault(pair =>
                    pair.Key.Id == field.m_dbId);

                if (string.IsNullOrEmpty(res.Key.Id))
                {
                    Debug.LogError($"Unable to resolve relation property with name {field.m_name} and id" +
                        $"{field.m_dbId}. Make sure to list every database in the config window.");
                    continue;
                }

                Fields.Add(new FieldDefinition()
                {
                    m_name = field.m_name,
                    m_type = $"{res.Value.Name.RemoveSpaces()}[]",    // Name of the type is the same as name of db.
                });
            }
        }
    }
}
