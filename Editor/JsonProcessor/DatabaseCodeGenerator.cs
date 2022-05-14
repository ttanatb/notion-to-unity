using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace NotionToUnity.Editor
{
    public class DatabaseCodeGenerator : MonoBehaviour
    {
        private const string FileExt = ".cs";

        private static string m_namespace = "Database";
        private static string m_editorScriptPath = "Scripts/Editor/Database";
        private static string m_structPath = "Scripts/Database";
        private static string m_soFormat = "{0}Database";

        public static void Init( string namespaceStr, string editorScriptPath, string structPath, string soFormat)
        {
            m_namespace = namespaceStr;
            m_editorScriptPath = editorScriptPath;
            m_structPath = structPath;
            m_soFormat = soFormat;
        }

        public static void Process(JObject json, out string soTypeName)
        {
            var dbProperty = new DatabaseProperty(json);
            var indent = new IndentConfig()
            {
                m_count = 4,
                m_indentType = IndentConfig.IndentType.Space,
            };
            GenerateStructFile(indent, dbProperty);
            GenerateScriptableObj(indent, dbProperty);
            soTypeName = string.Format(m_soFormat, dbProperty.Name);
            AssetDatabase.Refresh();
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

        private static void GenerateScriptableObj(IndentConfig indent, DatabaseProperty dbProperty)
        {
            // TODO: serializable dict

            string name = string.Format(m_soFormat, dbProperty.Name);
            var writer = GetStreamWriter(Application.dataPath, m_structPath, $"{name}{FileExt}");

            writer.WriteHeader(new[] { "System.Text", "System.Collections.Generic", "UnityEngine" },
                new[] { "CheckNamespace" });
            writer.WriteNamespace(m_namespace, ref indent);

            writer.WriteCode(indent,
                $"[CreateAssetMenu(fileName = \"{name}\", menuName = \"NotionToUnity/Db/{name}\", order = 0)]");
            writer.WriteCode(indent, $"public class {name} : ScriptableObject");
            writer.WriteOpenBracket(ref indent);

            writer.WriteCode(indent,
                $"public readonly Dictionary<string, {dbProperty.Name}> Data =" +
                $" new Dictionary<string,{dbProperty.Name}>();");
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

        private static void GenerateStructFile(IndentConfig indent, DatabaseProperty dbProperty)
        {
            var writer = GetStreamWriter(Application.dataPath, m_structPath, $"{dbProperty.Name}{FileExt}");

            writer.WriteHeader(new[] { "System.Text" }, new[] { "InconsistentNaming" });
            writer.WriteNamespace(m_namespace, ref indent);

            writer.WriteCode(indent, "[System.Serializable]");
            writer.WriteCode(indent, $"public struct {dbProperty.Name}");
            writer.WriteOpenBracket(ref indent);

            // Create enums
            foreach (var enums in dbProperty.Enums)
            {
                WriteEnum(writer, ref indent, enums);
                writer.WriteLine();
            }

            // Create fields
            foreach (var field in dbProperty.Fields)
            {
                writer.WriteCode(indent, $"public {field.m_type} {field.m_name};");
            }

            // Create ToString()
            writer.WriteCode(indent, "public override string ToString()");
            writer.WriteOpenBracket(ref indent);
            writer.WriteCode(indent, "var sb = new StringBuilder();");
            foreach (var field in dbProperty.Fields)
            {
                writer.WriteCode(indent, $"sb.AppendLine($\"\\t{field.m_name}: {{{field.m_name}}}\");");
            }
            writer.WriteCode(indent, "return sb.ToString();");
            writer.WriteClosingBracket(ref indent);

            writer.WriteAllClosingBracket(ref indent);
            writer.Flush();
            writer.Close();
        }

        private static void WriteEnum(StreamWriter writer,
            ref IndentConfig indent,
            NotionEnumDefinition enumDef)
        {
            writer.WriteCode(indent, $"public enum {enumDef.m_name} : int");
            writer.WriteOpenBracket(ref indent);

            writer.WriteCode(indent, "Invalid = 0,");
            foreach (var enumValue in enumDef.m_enums)
            {
                writer.WriteCode(indent, $"{enumValue.m_name} = {enumValue.m_value},");
            }

            writer.WriteClosingBracket(ref indent);
        }
    }
}
