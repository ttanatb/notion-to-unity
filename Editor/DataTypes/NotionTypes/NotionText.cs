using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using NotionToUnity.Utils;

namespace NotionToUnity.DataTypes
{
    /// <summary>
    /// Corresponds to the Rich Text or Title type in Notion, and it's just a wrapper around a string.
    /// </summary>
    public class NotionText : NotionType<string>
    {
        public NotionText(JToken property, Type localDbItemType) : base(property, localDbItemType)
        {
            Asserter.IsTrue(m_notionType == "rich_text" || m_notionType == "title");

            if (!property[m_notionType].Any())
            {
                Value = "";
                return;
            }

            Asserter.IsNotNull(property[m_notionType][0]);
            Asserter.IsNotNull(property[m_notionType][0]["text"]);
            Asserter.IsNotNull(property[m_notionType][0]["text"]["content"]);
            Value = property[m_notionType][0]["text"]["content"].Value<string>();
        }
    }
}
