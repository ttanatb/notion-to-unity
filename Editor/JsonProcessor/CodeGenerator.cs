using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using NotionToUnity.DataTypes;
using NotionToUnity.Editor;
using NotionToUnity.JsonProcessor.Types;
using NotionToUnity.Utils;

namespace NotionToUnity.JsonProcessor
{
    /// <summary>
    /// Generate script files with member definitions based on the database columns.
    /// </summary>
    public class CodeGenerator
    {
        private readonly NamingConvention m_namingConvention;
        private readonly ResourceGenerator m_resourceGenerator;

        /// <summary>
        /// Creates a code generator.
        /// </summary>
        /// <param name="namingConvention">Naming pattern</param>
        public CodeGenerator(NamingConvention namingConvention)
        {
            m_namingConvention = namingConvention;
            m_resourceGenerator = ResourceGenerator.Create(namingConvention);
        }

        /// <summary>
        /// Creates 
        /// </summary>
        /// <param name="databases"></param>
        /// <param name="dbPropertyJsons"></param>
        /// <param name="dbContentsJsons"></param>
        /// <returns></returns>
        public GeneratedResourceDef[] Process(List<NotionDatabaseDefinition> databases,
            Dictionary<string, JObject> dbPropertyJsons,
            Dictionary<string, List<JObject>> dbContentsJsons)
        {
            var results = new List<GeneratedResourceDef>();
            var indent = new IndentConfig()
            {
                m_count = m_namingConvention.IndentCount,
                m_indentType = m_namingConvention.IndentType,
            };

            Asserter.AreEqual(databases.Count, dbPropertyJsons.Count);
            Asserter.AreEqual(databases.Count, dbContentsJsons.Count);

            var processedDatabases = new Dictionary<NotionDatabaseDefinition, NotionTable>();
            
            // Process through all the databases
            foreach (var db in databases)
            {
                var dbProperty = dbPropertyJsons[db.Id];
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
                        Logger.LogError($"Unsupported db type {db.Type}");
                        break;
                }
                processedDatabases.Add(db, table);
            }

            // Resolves databases and generate code from all the processed databases
            foreach (var dbKeyValuePair in processedDatabases)
            {
                var dbDef = dbKeyValuePair.Key;
                var notionTable = dbKeyValuePair.Value;

                // Resolves relational databases
                notionTable.Resolve(processedDatabases);

                // Generate the actual code
                if (dbDef.Type == NotionDatabaseDefinition.DatabaseType.Table)
                {
                    var notionDb = (NotionDatabase)notionTable;
                    GenerateStructFile(indent, notionDb);
                    m_resourceGenerator.Generate(indent, notionDb);
                    results.Add(new GeneratedResourceDef
                    {
                        DbId = dbDef.Id,
                        ResourceTypeName = string.Format(m_namingConvention.ResourceClassNameFormat, notionTable.Name),
                    });
                }
                else if (dbDef.Type == NotionDatabaseDefinition.DatabaseType.EnumDef)
                {
                    var notionEnumDef = (NotionEnumDefinition)notionTable;
                    GenerateEnumFile(indent, notionEnumDef);
                }
            }

            return results.ToArray();
        }

        private void GenerateStructFile(IndentConfig indent, NotionDatabase notionDb)
        {
            var writer = GenericPath.GetStreamWriter(
                m_namingConvention.StructPath, notionDb.Name, GenericPath.ScriptExtension());

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

        private void GenerateEnumFile(IndentConfig indent, NotionEnumDefinition notionDb)
        {
            var writer = GenericPath.GetStreamWriter(
                m_namingConvention.StructPath,  notionDb.Name, GenericPath.ScriptExtension());

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
            // Write header of the enum
            writer.WriteCode(indent, "// Generated enum values look wacky, but they're MD5 hashes");
            writer.WriteCode(indent, "// of the enum IDs provided by Notion. This is to ensure that");
            writer.WriteCode(indent, "// enum values stay consistent even if the names were changed.");
            writer.WriteCode(indent, $"public enum {enumDef.m_name} : int");
            writer.WriteOpenBracket(ref indent);

            // TODO: make Invalid = 0 optional (also hoping the hasher doesn't hash to a 0)
            writer.WriteCode(indent, "Invalid = 0,");
            foreach (var enumValue in enumDef.m_enums)
            {
                writer.WriteCode(indent, $"{enumValue.m_name} = {enumValue.m_value},");
            }

            writer.WriteClosingBracket(ref indent);
        }
    }
}
