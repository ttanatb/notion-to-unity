using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NotionToUnity.DataTypes;
using NotionToUnity.Utils;

namespace NotionToUnity.JsonProcessor.Types
{
    /// <summary>
    /// Local representation of a notion table, to make code generation easier.
    /// </summary>
    public class NotionTable
    {
        /// <summary>
        /// The name of the database.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The list of enums in the table.
        /// </summary>
        public List<EnumDefinition> Enums { get; } = new List<EnumDefinition>();
        
        /// <summary>
        /// Checks the JSONs for correctness and gets the name of the table.
        /// </summary>
        /// <param name="propertyJson">The JSON defining the properties of the table</param>
        /// <param name="contentJsons">The JSON notating the contents in the table</param>
        protected NotionTable(JObject propertyJson, List<JObject> contentJsons)
        {
            if (propertyJson["object"].Value<string>() != "database")
            {
                Logger.LogError($"Invalid value in 'object' field: {propertyJson["object"]}");
                return;
            }

            foreach (var contentJson in contentJsons)
            {
                Asserter.IsNotNull(contentJson["object"]);
                if (contentJson["object"].Value<string>() == "list")
                    continue;
                
                Logger.LogError($"Invalid value in 'object' field: {contentJson["object"]}");
                return;
            }


            Name = GetName(propertyJson);
        }

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        /// <param name="propertyJson">Root JSON file for the database property</param>
        /// <returns>Name of the database</returns>
        private static string GetName(JObject propertyJson)
        {
            Asserter.IsNotNull(propertyJson["title"]);
            Asserter.IsNotNull(propertyJson["title"][0]);
            Asserter.IsNotNull(propertyJson["title"][0]["plain_text"]);
            string name = propertyJson["title"][0]["plain_text"].Value<string>();

            Asserter.IsNotNull(name);
            // TODO: set some config, not just hardcoded to replace 'Database'
            return name.Replace("Database", "").Trim().RemoveSpaces();
        }

        /// <summary>
        /// Helper method to get the name and type from a property JSON.
        /// </summary>
        /// <param name="currProperty">The JSON denoting the singular property</param>
        /// <param name="name">Name of the property</param>
        /// <param name="type">Type of the property</param>
        protected static void GetPropertyNameAndType(JToken currProperty, out string name, out string type)
        {
            Asserter.IsNotNull(currProperty["type"]);
            type = currProperty["type"].Value<string>();
            type = type.RemoveSpaces();

            Asserter.IsNotNull(currProperty["name"]);
            name = currProperty["name"].Value<string>();
            name = name.RemoveSpaces();
        }

        /// <summary>
        /// Resolves any relational databases to its correct types.
        /// </summary>
        /// <param name="processedDatabases"></param>
        public virtual void Resolve(Dictionary<NotionDatabaseDefinition, NotionTable> processedDatabases)
        {
            // Intentionally empty.
        }
    }
}
