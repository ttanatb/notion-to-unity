using System;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

// ReSharper disable once CheckNamespace
namespace NotionToUnity.Editor
{
    public class FieldPropagator : MonoBehaviour
    {
        private static IDictionary GetDictionary(ScriptableObject localDbSo)
        {
            var soType = localDbSo.GetType();
            var dictInfo = soType.GetField("Data");
            return (IDictionary)dictInfo.GetValue(localDbSo);
        }

        public static void Process(JObject json, ScriptableObject localDbSo)
        {
            if (json["object"].Value<string>() != "list")
            {
                Debug.LogError($"Invalid value in 'object' field: {json["object"]}");
                return;
            }

            var dictionary = GetDictionary(localDbSo);
            dictionary.Clear();
            var genericArgs = dictionary.GetType().GetGenericArguments();
            Type dbItemType = null;
            if (genericArgs.Length >= 2)
                dbItemType = genericArgs[1];
            else
            {
                // For serialized dictionary.
                Assert.IsNotNull(dictionary.GetType().BaseType);
                dbItemType = dictionary.GetType().BaseType?.GetGenericArguments()[1];
            }

            foreach (var row in json["results"])
            {
                var properties = row["properties"];
                Assert.IsNotNull(properties);

                // TODO: support title field with different name
                // get name to use as key-value
                Assert.IsNotNull(properties["Name"]);
                var name = new NotionText(properties["Name"], dbItemType);
                if (string.IsNullOrEmpty(name.Value))
                {
                    // TODO: check if other properties have value to create error messages
                    continue;
                }

                object dbItem = Activator.CreateInstance(dbItemType);
                foreach (var property in properties)
                {
                    // Not sure why, but need to access child here.
                    var currProperty = property.First;

                    Assert.IsNotNull(currProperty);
                    Assert.IsNotNull(currProperty["type"]);

                    // Create a notion property object to parse values.
                    var notionType = TypeMap.GetType(currProperty["type"].Value<string>());
                    object notionProperty = Activator.CreateInstance(notionType, currProperty, dbItemType);

                    var notionTypePropertyInfo = notionType.GetProperty("Value");
                    Assert.IsNotNull(notionTypePropertyInfo);

                    // Set each of the db item's field to the value from notion.
                    object value = notionTypePropertyInfo.GetValue(notionProperty);
                    string fieldName = currProperty.GetKey().RemoveSpaces();
                    var field = dbItemType.GetField(fieldName);

                    Assert.IsNotNull(field);
                    field.SetValue(dbItem, value);
                }

                dictionary[name.Value] = dbItem;
            }

            Debug.Log(localDbSo);

            EditorUtility.SetDirty(localDbSo);
            Undo.RecordObject(localDbSo, "Updated data");
            AssetDatabase.SaveAssets();
        }
    }
}

