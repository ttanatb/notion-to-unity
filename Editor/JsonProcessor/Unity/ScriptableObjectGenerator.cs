#if UNITY_EDITOR
using NotionToUnity.Editor;
using NotionToUnity.JsonProcessor.Types;
using NotionToUnity.Utils;
using UnityEditor;

namespace NotionToUnity.JsonProcessor
{
    /// <summary>
    /// Code generator for ScriptableObject files (both for the table definitions and for the local database itself).
    /// </summary>
    public class ScriptableObjectGenerator : ResourceGenerator
    {
        /// <summary>
        /// File extension for the resource object (the local table or ScriptableObject asset).
        /// </summary>
        private const string SoAssetFileExt = ".asset";

        /// <summary>
        /// Creates an instance of a scriptable object generator.
        /// </summary>
        /// <param name="namingConvention">The naming convention for the generated scripts</param>
        public ScriptableObjectGenerator(NamingConvention namingConvention) : base(namingConvention)
        {
            m_namingConvention = namingConvention;
        }

        /// <summary>
        /// Generates the ScriptableObject script file and asset file.
        /// </summary>
        /// <param name="indent">The indentation pattern for the script file</param>
        /// <param name="notionDb">The corresponding notion database</param>
        public override void Generate(IndentConfig indent, NotionDatabase notionDb)
        {
            GenerateSoFile(indent, notionDb);
            GenerateSoAsset(notionDb);
        }
        
        private void GenerateSoFile(IndentConfig indent, NotionTable notionDb)
        {
            GenerateSerializableDictPropertyDrawer(indent, notionDb);
            string name = string.Format(m_namingConvention.ResourceClassNameFormat, notionDb.Name);
            var writer = GenericPath.GetStreamWriter(
                m_namingConvention.StructPath, name, GenericPath.ScriptExtension());

            writer.WriteHeader(new[] { "System.Text", "System.Collections.Generic", "UnityEngine" },
                new[] { "CheckNamespace" });
            writer.WriteNamespace(m_namingConvention.Namespace, ref indent);

            // Define SerializedDict
            writer.WriteCode(indent, "[System.Serializable]");
            writer.WriteCode(indent, $"public class Id{notionDb.Name}Dictionary : " +
                $"SerializableDictionary<string, {notionDb.Name}> {{ }}");
            writer.WriteLine();

            writer.WriteCode(indent,
                $"[CreateAssetMenu(fileName = \"{name}\", menuName = \"NotionToUnity/Db/{name}\", order = 0)]");
            writer.WriteCode(indent, $"public class {name} : ScriptableObject");
            writer.WriteOpenBracket(ref indent);

            // Define property
            // TODO: make this configurable
            writer.WriteCode(indent, "[field: SerializeField]");
            writer.WriteCode(indent, $"public Id{notionDb.Name}Dictionary Data {{ get; private set; }}");
            writer.WriteLine();

            // toString method
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

        // TODO: Make the dependency on the serializable dict optional
        private void GenerateSerializableDictPropertyDrawer(IndentConfig indent, NotionTable notionDb)
        {
            string dictName = $"Id{notionDb.Name}Dictionary";
            string propertyDrawer = $"{dictName}PropertyDrawer";
            var writer = GenericPath.GetStreamWriter(
                m_namingConvention.EditorScriptPath, propertyDrawer, GenericPath.ScriptExtension());

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
        
        private void GenerateSoAsset(NotionTable notionDb)
        {
            // God I hope nothing in the .asset file changes
            string name = string.Format(m_namingConvention.ResourceClassNameFormat, notionDb.Name);
            string guid = AssetDatabase.AssetPathToGUID(
                $"Assets/{m_namingConvention.StructPath}/{name}{GenericPath.ScriptExtension()}");
            var writer = GenericPath.GetStreamWriter(
                m_namingConvention.ResourceFilePath, name, SoAssetFileExt);
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
#endif
