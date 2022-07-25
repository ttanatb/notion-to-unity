using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using NotionToUnity.DataTypes;
using NotionToUnity.Utils;

namespace NotionToUnity.JsonProcessor.Types
{
    /// <summary>
    /// Local representation of an enum database.
    /// </summary>
    public class NotionEnumDefinition : NotionTable
    {
        /// <summary>
        /// Creates an enum list from the table property and the table contents.
        /// </summary>
        /// <param name="propertyJson">The JSON defining the table properties</param>
        /// <param name="contentJsons">The JSON defining table contents</param>
        public NotionEnumDefinition(JObject propertyJson, List<JObject> contentJsons) : base(propertyJson, contentJsons)
        {
            // Check expected columns
            bool hasTitle = false;
            bool hasComment = false;
            Asserter.IsNotNull(propertyJson["properties"]);
            foreach (var property in propertyJson["properties"])
            {
                // Not sure why, but need to access child here.
                var currProperty = property.First;
                Asserter.IsNotNull(currProperty);

                GetPropertyNameAndType(currProperty, out string name, out string type);

                if (type == "title")
                    hasTitle = true;
                else if (type == "rich_text" && name.ToLower() == "comment")
                    hasComment = true;
            }

            if (!hasTitle)
            {
                Logger.LogError($"Enum Definition Database ({Name}) is missing required title field.");
                return;
            }
            if (!hasComment)
            {
                Logger.Log($"Enum Definition Database ({Name}) is missing optional comment field.");
                return;
            } 

            // Hasher? I barely even know 'er!
            using (var hasher = MD5.Create())
            {
                var enumValues = new List<EnumValue>();

                // Convert each row into an enum value
                foreach (var contentJson in contentJsons)
                {
                    Asserter.IsNotNull(contentJson["results"]);
                    foreach (var row in contentJson["results"])
                    {
                        var properties = row["properties"];
                        Asserter.IsNotNull(properties);
                        Asserter.IsNotNull(properties["Name"]);
                        var name = new NotionText(properties["Name"], null);
                        if (string.IsNullOrEmpty(name.Value))
                            continue;

                        var comment = new NotionText(properties["Comment"], null);

                        Asserter.IsNotNull(row["id"]);
                        string id = row["id"].Value<string>();

                        Asserter.IsNotNull(id);
                        int value = BitConverter.ToInt32(
                            hasher.ComputeHash(Encoding.UTF8.GetBytes(id)), 0);

                        enumValues.Add(new EnumValue()
                        {
                            m_comment = comment.Value,
                            m_name = name.Value,
                            m_value = value,
                        });
                    }
                }

                Enums.Add(new EnumDefinition()
                {
                    m_enums = enumValues.ToArray(),
                    m_name = Name,
                });
            }
        }
    }
}
