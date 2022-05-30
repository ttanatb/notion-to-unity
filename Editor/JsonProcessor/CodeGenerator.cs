using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

// ReSharper disable once CheckNamespace
namespace NotionToUnity.Editor
{
    public class CodeGenerator : MonoBehaviour
    {
        private const string ScriptFileExt = ".cs";
        private const string SoAssetFileExt = ".asset";
        private static NamingConvention m_namingConvention;

        public static void Init(NamingConvention namingConvention)
        {
            m_namingConvention = namingConvention;
        }

        public static List<GeneratedSo> Process(List<NotionDatabaseDefinition> databases,
            List<JObject> dbPropertyJsons,
            Dictionary<string, JObject> dbContentsJsons)
        {
            var results = new List<GeneratedSo>();

            // TODO: this should probably be elsewhere
            var indent = new IndentConfig()
            {
                m_count = 4,
                m_indentType = IndentConfig.IndentType.Space,
            };


            Assert.AreEqual(databases.Count, dbPropertyJsons.Count);
            Assert.AreEqual(databases.Count, dbContentsJsons.Count);

            var processedDatabases = new Dictionary<NotionDatabaseDefinition, NotionTable>();
            for (int i = 0; i < databases.Count; i++)
            {
                var db = databases[i];
                var dbProperty = dbPropertyJsons[i];
                var dbContents = dbContentsJsons[db.Id];

                NotionTable table = null;
                switch (db.Type)
                {
                    case NotionDatabaseDefinition.DatabaseType.Table:
                        table = new NotionDatabase(dbProperty, dbContents);
                        break;
                    case NotionDatabaseDefinition.DatabaseType.EnumDef:
                        table = new NotionEnumDefinition(dbProperty, dbContents);
                        break;
                    default:
                        Debug.LogError($"Unsupported db type {db.Type}");
                        break;
                }
                processedDatabases.Add(db, table);
            }

            foreach (var dbKeyValuePair in processedDatabases)
            {
                var dbDef = dbKeyValuePair.Key;
                var notionTable = dbKeyValuePair.Value;

                notionTable.Resolve(processedDatabases);

                if (dbDef.Type == NotionDatabaseDefinition.DatabaseType.Table)
                {
                    var notionDb = (NotionDatabase)notionTable;
                    GenerateStructFile(indent, notionDb);
                    GenerateScriptableObj(indent, notionDb);
                    GenerateSoAsset(notionDb);
                    results.Add(new GeneratedSo
                    {
                        DbId = dbDef.Id,
                        SoTypeName = string.Format(m_namingConvention.SoFormat, notionTable.Name),
                    });
                }
                else if (dbDef.Type == NotionDatabaseDefinition.DatabaseType.EnumDef)
                {
                    var notionEnumDef = (NotionEnumDefinition)notionTable;
                    GenerateEnumFile(indent, notionEnumDef);
                }
            }

            return results;
        }

        private static StreamWriter GetStreamWriter(string basePath, string path, string file)
        {
            string totalPath = basePath;
            foreach (string dir in path.Split('/'))
            {
                totalPath = Path.Combine(totalPath, dir);
                if (!Directory.Exists(totalPath))
                    Directory.CreateDirectory(totalPath);
            }
            totalPath = Path.Combine(totalPath, file);

            return File.Exists(totalPath) ? new StreamWriter(totalPath, false) : File.CreateText(totalPath);
        }

        private static void GenerateSerializableDictPropertyDrawer(IndentConfig indent, NotionDatabase notionDb)
        {
            string dictName = $"Id{notionDb.Name}Dictionary";
            string propertyDrawer = $"{dictName}PropertyDrawer";
            var writer = GetStreamWriter(Application.dataPath,
                m_namingConvention.EditorScriptPath, $"{propertyDrawer}{ScriptFileExt}");

            writer.WriteLine("#if UNITY_EDITOR");
            writer.WriteHeader(new[] { "UnityEditor" },
                new[] { "CheckNamespace" });
            writer.WriteNamespace(m_namingConvention.Namespace, ref indent);

            writer.WriteLine($"[CustomPropertyDrawer(typeof({dictName}))]");
            writer.WriteLine($"public class {propertyDrawer} : SerializableDictionaryPropertyDrawer {{ }}");
            writer.WriteAllClosingBracket(ref indent);

            writer.WriteLine("#endif // UNITY_EDITOR");
            writer.Flush();
            writer.Close();
        }

        private static void GenerateScriptableObj(IndentConfig indent, NotionDatabase notionDb)
        {
            GenerateSerializableDictPropertyDrawer(indent, notionDb);
            string name = string.Format(m_namingConvention.SoFormat, notionDb.Name);
            var writer = GetStreamWriter(
                Application.dataPath, m_namingConvention.StructPath, $"{name}{ScriptFileExt}");

            writer.WriteHeader(new[] { "System.Text", "System.Collections.Generic", "UnityEngine" },
                new[] { "CheckNamespace" });
            writer.WriteNamespace(m_namingConvention.Namespace, ref indent);

            writer.WriteCode(indent, "[System.Serializable]");
            writer.WriteCode(indent, $"public class Id{notionDb.Name}Dictionary : " +
                $"SerializableDictionary<string, {notionDb.Name}> {{ }}");

            writer.WriteCode(indent,
                $"[CreateAssetMenu(fileName = \"{name}\", menuName = \"NotionToUnity/Db/{name}\", order = 0)]");
            writer.WriteCode(indent, $"public class {name} : ScriptableObject");
            writer.WriteOpenBracket(ref indent);

            writer.WriteCode(indent,
                $"public Id{notionDb.Name}Dictionary Data =" +
                $" new Id{notionDb.Name}Dictionary();");
            writer.WriteLine();

            writer.WriteCode(indent, "public override string ToString()");
            writer.WriteOpenBracket(ref indent);

            writer.WriteCode(indent, "var sb = new StringBuilder();");
            writer.WriteCode(indent, "foreach (var entry in Data)");
            writer.WriteOpenBracket(ref indent);
            writer.WriteCode(indent, "sb.AppendLine($\"{entry.Key}:\\n{entry.Value}\");");
            writer.WriteClosingBracket(ref indent);
            writer.WriteCode(indent, "return sb.ToString();");

            writer.WriteAllClosingBracket(ref indent);
            writer.Flush();
            writer.Close();
        }

        private static void GenerateStructFile(IndentConfig indent, NotionDatabase notionDb)
        {
            var writer = GetStreamWriter(
                Application.dataPath, m_namingConvention.StructPath, $"{notionDb.Name}{ScriptFileExt}");

            writer.WriteHeader(new[] { "System.Text" }, new[] { "InconsistentNaming" });
            writer.WriteNamespace(m_namingConvention.Namespace, ref indent);

            writer.WriteCode(indent, "[System.Serializable]");
            writer.WriteCode(indent, $"public struct {notionDb.Name}");
            writer.WriteOpenBracket(ref indent);

            // Create enums
            foreach (var enums in notionDb.Enums)
            {
                WriteEnum(writer, ref indent, enums);
                writer.WriteLine();
            }

            // Create fields
            foreach (var field in notionDb.Fields)
            {
                writer.WriteCode(indent, $"public {field.m_type} {field.m_name};");
            }
            writer.WriteLine();

            // Create ToString()
            writer.WriteCode(indent, "public override string ToString()");
            writer.WriteOpenBracket(ref indent);
            writer.WriteCode(indent, "var sb = new StringBuilder();");
            foreach (var field in notionDb.Fields)
            {
                writer.WriteCode(indent, $"sb.AppendLine($\"\\t{field.m_name}: {{{field.m_name}}}\");");
            }
            writer.WriteCode(indent, "return sb.ToString();");
            writer.WriteClosingBracket(ref indent);

            writer.WriteAllClosingBracket(ref indent);
            writer.Flush();
            writer.Close();
        }

        private static void GenerateEnumFile(IndentConfig indent, NotionEnumDefinition notionDb)
        {
            var writer = GetStreamWriter(
                Application.dataPath, m_namingConvention.StructPath, $"{notionDb.Name}{ScriptFileExt}");

            writer.WriteHeader(new[] { "System.Text" }, null);
            writer.WriteNamespace(m_namingConvention.Namespace, ref indent);

            // Create enums
            foreach (var enums in notionDb.Enums)
            {
                WriteEnum(writer, ref indent, enums);
                writer.WriteLine();
            }

            writer.WriteAllClosingBracket(ref indent);
            writer.Flush();
            writer.Close();
        }

        private static void WriteEnum(StreamWriter writer,
            ref IndentConfig indent,
            EnumDefinition enumDef)
        {
            writer.WriteCode(indent, "// Generated enum values look wacky, but they're MD5 hashes");
            writer.WriteCode(indent, "// of the enum IDs provided by Notion. This is to ensure that");
            writer.WriteCode(indent, "// enum values stay consistent even if the names were changed.");
            writer.WriteCode(indent, $"public enum {enumDef.m_name} : int");
            writer.WriteOpenBracket(ref indent);

            writer.WriteCode(indent, "Invalid = 0,");
            foreach (var enumValue in enumDef.m_enums)
            {
                writer.WriteCode(indent, $"{enumValue.m_name} = {enumValue.m_value},");
            }

            writer.WriteClosingBracket(ref indent);
        }

        private static void GenerateSoAsset(NotionDatabase notionDb)
        {
            string name = string.Format(m_namingConvention.SoFormat, notionDb.Name);
            string guid = AssetDatabase.AssetPathToGUID($"Assets/{m_namingConvention.StructPath}/{name}{ScriptFileExt}");
            var writer = GetStreamWriter(
                Application.dataPath, m_namingConvention.SoPath, $"{name}{SoAssetFileExt}");
            writer.WriteLine("%YAML 1.1");
            writer.WriteLine("%TAG !u! tag:unity3d.com,2011:");
            writer.WriteLine("--- !u!114 &11400000");
            writer.WriteLine("MonoBehaviour:");
            writer.WriteLine("  m_ObjectHideFlags: 0");
            writer.WriteLine("  m_CorrespondingSourceObject: {fileID: 0}");
            writer.WriteLine("  m_PrefabInstance: {fileID: 0}");
            writer.WriteLine("  m_PrefabAsset: {fileID: 0}");
            writer.WriteLine("  m_GameObject: {fileID: 0}");
            writer.WriteLine("  m_Enabled: 1");
            writer.WriteLine("  m_EditorHideFlags: 0");
            writer.WriteLine($"  m_Script: {{fileID: 11500000, guid: {guid}, type: 3}}");
            writer.WriteLine($"  m_Name: {name}");
            writer.WriteLine("  m_EditorClassIdentifier: ");
            writer.Flush();
            writer.Close();
        }
    }
}
