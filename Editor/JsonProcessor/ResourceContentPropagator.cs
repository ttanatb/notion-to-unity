using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine.Assertions;
using NotionToUnity.DataTypes;
using NotionToUnity.Utils;

namespace NotionToUnity.JsonProcessor
{
    /// <summary>
    /// Propagates the content of the resource object.
    /// </summary>
    public static class ResourceContentPropagator 
    {
        /// <summary>
        /// Processes the content JSON and propagates the dictionary in the resource file
        /// </summary>
        /// <param name="contentJsons">JSON list denoting the table contents</param>
        /// <param name="resourceObject">Object of the resource file</param>
        public static void Process(List<JObject> contentJsons, object resourceObject)
        {
            foreach (var page in contentJsons.Where(page => page["object"].Value<string>() != "list"))
            {
                Logger.LogError($"Invalid value in 'object' field: {page["object"]}");
                return;
            }

            var dictionary = GetDictionary(resourceObject);
            dictionary.Clear();
            var genericArgs = dictionary.GetType().GetGenericArguments();
            
            // Type of the local database.
            Type localDbItemType = null;
            if (genericArgs.Length >= 2)
                localDbItemType = genericArgs[1];
            else
            {
                // For serialized dictionary.
                Assert.IsNotNull(dictionary.GetType().BaseType);
                localDbItemType = dictionary.GetType().BaseType?.GetGenericArguments()[1];
            }

            foreach (var page in contentJsons)
            {
                ProcessPage(page, localDbItemType, dictionary);
            }

            Logger.Log($"Completed field propagation: localDbSo");
            GenericSave.SaveSo(resourceObject);
        }
        
        /// <summary>
        /// Get the dictionary property
        /// </summary>
        /// <param name="resourceObject">Object of the resource file</param>
        /// <returns>The dictionary object</returns>
        private static IDictionary GetDictionary(object resourceObject)
        {
            var soType = resourceObject.GetType();
            var dictInfo = soType.GetProperty("Data");
            Assert.IsNotNull(dictInfo);
            return (IDictionary)dictInfo.GetValue(resourceObject);
        }
        
        /// <summary>
        /// Process each entry in the database
        /// </summary>
        /// <param name="json">JSON for each page</param>
        /// <param name="localDbItemType">Type of the object in the database</param>
        /// <param name="dictionary">Dictionary to propagate</param>
        private static void ProcessPage(JObject json, Type localDbItemType, IDictionary dictionary)
        {
            foreach (var row in json["results"])
            {
                var properties = row["properties"];
                Assert.IsNotNull(properties);

                // TODO: support title field with different name
                // get name to use as key-value
                Assert.IsNotNull(properties["Name"]);
                var name = new NotionText(properties["Name"], localDbItemType);
                if (string.IsNullOrEmpty(name.Value))
                {
                    // TODO: check if other properties have value to create error messages
                    continue;
                }

                Asserter.IsNotNull(localDbItemType);
                object dbItem = Activator.CreateInstance(localDbItemType);
                foreach (var property in properties)
                {
                    // Not sure why, but need to access child here.
                    var currProperty = property.First;

                    Assert.IsNotNull(currProperty);
                    Assert.IsNotNull(currProperty["type"]);

                    // Create a notion property object to parse values.
                    var notionType = TypeMap.GetType(currProperty["type"].Value<string>());
                    object notionProperty = Activator.CreateInstance(notionType, currProperty, localDbItemType);

                    var notionTypePropertyInfo = notionType.GetProperty("Value");
                    Assert.IsNotNull(notionTypePropertyInfo);

                    // Set each of the db item's field to the value from notion.
                    object value = notionTypePropertyInfo.GetValue(notionProperty);
                    string fieldName = currProperty.GetKey().RemoveSpaces();
                    var field = localDbItemType.GetField(fieldName);

                    Assert.IsNotNull(field);
                    field.SetValue(dbItem, value);
                }

                dictionary[name.Value] = dbItem;
            }
        }
    }
}

