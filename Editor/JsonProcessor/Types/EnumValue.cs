namespace NotionToUnity.JsonProcessor.Types
{
    /// <summary>
    /// Describes one value of an enum
    /// </summary>
    public struct EnumValue
    {
        /// <summary>
        /// The name of the value
        /// </summary>
        public string m_name;
        
        /// <summary>
        /// The actual value itself (usually an int)
        /// </summary>
        public int m_value;
        
        /// <summary>
        /// Comment associated with the value
        /// </summary>
        public string m_comment;
    }
}
