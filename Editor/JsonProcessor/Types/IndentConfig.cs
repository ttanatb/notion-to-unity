namespace NotionToUnity.JsonProcessor.Types
{
    /// <summary>
    /// Used by the code generator to configure the script indentation
    /// </summary>
    public struct IndentConfig
    {
        /// <summary>
        /// The count of tabs or spaces for one indent
        /// </summary>
        public int m_count;
        
        /// <summary>
        /// The character used for an indent
        /// </summary>
        public IndentType m_indentType;
        
        /// <summary>
        /// The type of indent, corresponds to a character that's used.
        /// </summary>
        public enum IndentType 
        {
            /// <summary>
            /// Corresponds to the space character (' ')
            /// </summary>
            Space,
            
            /// <summary>
            /// Corresponds ot the tab character ('\t')
            /// </summary>
            Tab
        }
        
        /// <summary>
        /// The count of nested brackets in the current line of the script.
        /// </summary>
        public int m_bracketCount;

        /// <summary>
        /// Generates the indentation preceding a line of code.
        /// </summary>
        /// <returns>The indentation preceding a line of code.</returns>
        public override string ToString()
        {
            char space = m_indentType == IndentType.Space ? ' ' : '\t';
            return new string(space, m_bracketCount * m_count);
        }
    }
}

