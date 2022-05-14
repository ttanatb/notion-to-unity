// ReSharper disable once CheckNamespace
namespace NotionToUnity
{
    public struct IndentConfig
    {
        public int m_count;
        public IndentType m_indentType;
        public int m_bracketCount;
        public enum IndentType
        {
            Space,
            Tab
        }

        public override string ToString()
        {
            char space = m_indentType == IndentType.Space ? ' ' : '\t';
            return new string(space, m_bracketCount * m_count);
        }
    }
}

