using NotionToUnity.JsonProcessor.Types;

namespace NotionToUnity.JsonProcessor
{
    /// <summary>
    /// Abstract class for creating the resource generator for each engine
    /// </summary>
    public abstract class ResourceGenerator
    {
        protected NamingConvention m_namingConvention;

        /// <summary>
        /// Creates a resource generator for the specific engine
        /// </summary>
        /// <param name="namingConvention"></param>
        /// <returns></returns>
        public static ResourceGenerator Create(NamingConvention namingConvention)
        {
            #if UNITY_EDITOR
            return new ScriptableObjectGenerator(namingConvention);
            #else
            throw new NotImplementedException();
            #endif
        }
        
        protected ResourceGenerator(NamingConvention namingConvention)
        {
            // Intentionally empty
        }

        public abstract void Generate(IndentConfig indent, NotionDatabase notionDb);
    }
}
