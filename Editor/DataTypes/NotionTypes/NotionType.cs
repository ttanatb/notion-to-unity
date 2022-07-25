using System;
using Newtonsoft.Json.Linq;
using NotionToUnity.Utils;

namespace NotionToUnity.DataTypes
{
    /// <summary>
    /// Base type for all local representations of a type in Notion's database.
    /// </summary>
    /// <typeparam name="T">The data used to locally store its value</typeparam>
    public class NotionType<T>
    {
        /// <summary>
        /// The value of the variable that this type wraps around.
        /// </summary>
        public T Value { get; protected set; }

        /// <summary>
        /// Name of the variable.
        /// </summary>
        protected readonly string m_name;
        
        /// <summary>
        /// Name of the variable type.
        /// </summary>
        protected readonly string m_notionType;

        /// <summary>
        /// Creates a local Notion type and ensures that expected properties are present.
        /// </summary>
        /// <param name="property">The JSON object representing the property</param>
        /// <param name="localDbItemType">Type of the local database item</param>
        protected NotionType(JToken property, Type localDbItemType)
        {
            m_name = property["name"] == null ? property.GetKey() : property["name"].Value<string>();
            m_notionType = property["type"].Value<string>();
            Asserter.IsNotNull(m_notionType);
            Asserter.IsNotNull(property[m_notionType]);
        }
    }
}
