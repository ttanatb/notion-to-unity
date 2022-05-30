using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json.Linq;

// ReSharper disable once CheckNamespace
namespace NotionToUnity.Editor
{
    public static class ClassExtensions
    {
        public static void WriteHeader(this StreamWriter writer,
            IEnumerable<string> includes,
            IEnumerable<string> disableWarnings)
        {
            if (includes != null)
            {
                foreach (string s in includes)
                    writer.WriteLine($"using {s};");

                writer.WriteLine();
            }

            if (disableWarnings != null)
            {
                foreach (string s in disableWarnings)
                    writer.WriteLine($"// Resharper disable {s};");

                writer.WriteLine();
            }

            writer.WriteLine("// Generated code from NotionToUnity");
            writer.WriteLine();
        }

        public static void WriteNamespace(this StreamWriter writer, string namespaceStr, ref IndentConfig indent)
        {
            if (string.IsNullOrEmpty(namespaceStr))
                return;

            writer.WriteLine($"namespace {namespaceStr}");
            writer.WriteOpenBracket(ref indent);
        }

        public static void WriteCode(this StreamWriter writer, IndentConfig indent, string line)
        {
            writer.Write(indent.ToString());
            writer.WriteLine(line);
        }

        public static void WriteOpenBracket(this StreamWriter writer, ref IndentConfig indent)
        {
            writer.WriteLine($"{indent.ToString()}{{");
            indent.m_bracketCount++;
        }

        public static void WriteClosingBracket(this StreamWriter writer, ref IndentConfig indent)
        {
            indent.m_bracketCount--;
            writer.WriteLine($"{indent.ToString()}}}");
        }

        public static void WriteAllClosingBracket(this StreamWriter writer, ref IndentConfig indent)
        {
            while (indent.m_bracketCount > 0)
            {
                writer.WriteClosingBracket(ref indent);
            }
        }

        public static ScriptableObject[] GetAllInstances(string typeName)
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeName);
            var scriptableObjs = new ScriptableObject[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                scriptableObjs[i] = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            }

            return scriptableObjs;

        }

        public static string GetKey(this JToken token)
        {
            string path = token.Path;
            if (path[path.Length - 1] == ']')
            {
                int index = path.LastIndexOf('[');
                string key = path.Substring(index);

                // Trim first and last 2 chars ("['key']" -> "key")
                return key.Substring(2, key.Length - 4);
            }
            else
            {
                int index = path.LastIndexOf('.');
                return path.Substring(index + 1);
            }
        }

        public static string RemoveSpaces(this string s)
        {
            return s.Replace(" ", "");
        }
    }
}
