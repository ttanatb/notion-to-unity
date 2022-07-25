namespace NotionToUnity.JsonProcessor.Types
{
    /// <summary>
    /// Defines an enum and its list of values.
    /// </summary>
    public struct EnumDefinition
    {
        /// <summary>
        /// The name of the enum
        /// </summary>
        public string m_name;
        
        /// <summary>
        /// The list of values of the enum
        /// </summary>
        public EnumValue[] m_enums;
    }
}
