using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using NotionToUnity.DataTypes;
using NotionToUnity.Utils;

namespace NotionToUnity.JsonProcessor.Types
{
    /// <summary>
    /// Local representation of a notion database.
    /// </summary>
    public class NotionDatabase : NotionTable
    {
        /// <summary>
        /// List of fields in this database.
        /// </summary>
        public List<FieldDefinition> Fields { get; } = new List<FieldDefinition>();

        /// <summary>
        /// List of unresolved fields in this database.
        /// </summary>
        private readonly List<UnresolvedFieldDefinition> m_unresolvedFields = new List<UnresolvedFieldDefinition>();

        /// <summary>
        /// Creates the database from the JSONs defining the columns and the rows of the table.
        /// </summary>
        /// <param name="propertyJson">JSON defining the table property</param>
        /// <param name="contentJsons">List of JSONs defining table entries</param>
        public NotionDatabase(JObject propertyJson, List<JObject> contentJsons) : base(propertyJson, contentJsons)
        {
            // Hasher? I barely even know 'er!
            using (var hasher = MD5.Create())
            {
                Asserter.IsNotNull(propertyJson["properties"]);
                foreach (var property in propertyJson["properties"])
                {
                    // Not sure why, but need to access child here.
                    var currProperty = property.First;
                    Asserter.IsNotNull(currProperty);

                    GetPropertyNameAndType(currProperty, out string name, out string type);
                    if (type == "select")
                        CreateSelectProperty(currProperty, hasher, name);
                    else if (type == "title" || type == "rich_text")
                        CreateTextProperty(name);
                    else if (type == "relation")
                        CreateRelationProperty(currProperty, name, type);
                    else
                        Logger.LogWarning($"Encountered unsupported property type: {type}");
                }

                hasher.Dispose();
            }
        }
        
        /// <summary>
        /// Resolve all relation types with the processed databases.
        /// </summary>
        /// <param name="processedDatabases">All the processed databases</param>
        public override void Resolve(Dictionary<NotionDatabaseDefinition, NotionTable> processedDatabases)
        {
            foreach (var field in m_unresolvedFields)
            {
                // var res = default(KeyValuePair<NotionDatabaseDefinition, NotionDatabase>);
                var res = processedDatabases.FirstOrDefault(pair =>
                    pair.Key.Id == field.m_dbId);

                if (string.IsNullOrEmpty(res.Key.Id))
                {
                    Logger.LogError($"Unable to resolve relation property with name {field.m_name} and id" +
                        $"{field.m_dbId}. Make sure to list every database in the config window.");
                    continue;
                }

                Fields.Add(new FieldDefinition()
                {
                    m_name = field.m_name,
                    m_type = $"{res.Value.Name.RemoveSpaces()}[]", // Name of the type is the same as name of db.
                });
            }
        }

        private void CreateSelectProperty(JToken currProperty, HashAlgorithm hasher, string name)
        {
            // Create enum values
            Asserter.IsNotNull(currProperty["select"]);
            var opts = currProperty["select"]["options"];

            Asserter.IsNotNull(opts);
            var enums = new List<EnumValue>();

            foreach (var opt in opts)
            {
                Asserter.IsNotNull(opt["name"]);
                string optName = opt["name"].Value<string>();

                Asserter.IsNotNull(opt["id"]);
                string id = opt["id"].Value<string>();
                Asserter.IsNotNull(id);

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

        private void CreateTextProperty(string name)
        {
            Fields.Add(new FieldDefinition()
            {
                m_name = name,
                m_type = "string"
            });
        }

        private void CreateRelationProperty(JToken currProperty, string name, string type)
        {
            Asserter.IsNotNull(currProperty[type]);
            Asserter.IsNotNull(currProperty[type]["database_id"]);

            string dbId = currProperty[type]["database_id"].Value<string>();
            Asserter.IsNotNull(dbId);
            m_unresolvedFields.Add(new UnresolvedFieldDefinition()
            {
                m_name = name,
                m_dbId = dbId.Replace("-", ""), // Notion DB can have extra '-' we don't keep track of
            });
        }
    }
}
