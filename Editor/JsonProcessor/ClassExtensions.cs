using System.Collections.Generic;
using System.IO;
using NotionToUnity.JsonProcessor.Types;

namespace NotionToUnity.Editor
{
    public static class ClassExtensions
    {
        /// <summary>
        /// Writes the header of a script file
        /// </summary>
        /// <param name="writer">The stream writer</param>
        /// <param name="includes">Namespaces to include</param>
        /// <param name="disableWarnings">Warnings to disable (for Rider)</param>
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

        /// <summary>
        /// Writes the namespace to the script file
        /// </summary>
        /// <param name="writer">The stream writer</param>
        /// <param name="namespaceStr">The namespace</param>
        /// <param name="indent">Used to track indents</param>
        public static void WriteNamespace(this StreamWriter writer, string namespaceStr, ref IndentConfig indent)
        {
            if (string.IsNullOrEmpty(namespaceStr))
                return;

            writer.WriteLine($"namespace {namespaceStr}");
            writer.WriteOpenBracket(ref indent);
        }

        /// <summary>
        /// Writes a line of code (make sure to include a ';' if needed)
        /// </summary>
        /// <param name="writer">The stream writer</param>
        /// <param name="indent">Amount of indent</param>
        /// <param name="line">The line of code</param>
        public static void WriteCode(this StreamWriter writer, IndentConfig indent, string line)
        {
            writer.Write(indent.ToString());
            writer.WriteLine(line);
        }

        /// <summary>
        /// Writes an open bracket
        /// </summary>
        /// <param name="writer">The stream writer</param>
        /// <param name="indent">Amount of indent</param>
        public static void WriteOpenBracket(this StreamWriter writer, ref IndentConfig indent)
        {
            writer.WriteLine($"{indent.ToString()}{{");
            indent.m_bracketCount++;
        }

        /// <summary>
        /// Writes a closed bracket
        /// </summary>
        /// <param name="writer">The stream writer</param>
        /// <param name="indent">Amount of indent</param>
        public static void WriteClosingBracket(this StreamWriter writer, ref IndentConfig indent)
        {
            indent.m_bracketCount--;
            writer.WriteLine($"{indent.ToString()}}}");
        }

        /// <summary>
        /// CLoses all remaining brackets in the file.
        /// </summary>
        /// <param name="writer">The stream writer</param>
        /// <param name="indent">Amount of indent</param>
        public static void WriteAllClosingBracket(this StreamWriter writer, ref IndentConfig indent)
        {
            while (indent.m_bracketCount > 0)
            {
                writer.WriteClosingBracket(ref indent);
            }
        }
    }
}
