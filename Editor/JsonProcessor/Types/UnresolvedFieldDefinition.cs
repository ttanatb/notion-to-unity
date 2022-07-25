namespace NotionToUnity.JsonProcessor.Types
{
    /// <summary>
    /// Defines a field with an unresolved type (relational database).
    /// </summary>
    public struct UnresolvedFieldDefinition
    {
        /// <summary>
        /// Name of the field.
        /// </summary>
        public string m_name;
        
        /// <summary>
        /// Database ID of the type to resolve to.
        /// </summary>
        public string m_dbId;
    }
}
