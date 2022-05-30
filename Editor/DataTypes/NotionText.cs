using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine.Assertions;

// ReSharper disable once CheckNamespace
namespace NotionToUnity.Editor
{
    public class NotionText : NotionType<string>
    {
        public NotionText(JToken property, Type dbItemType) : base(property, dbItemType)
        {
            Assert.IsTrue(m_notionType == "rich_text" || m_notionType == "title");

            if (!property[m_notionType].Any())
            {
                Value = "";
                return;
            }

            Assert.IsNotNull(property[m_notionType][0]);
            Assert.IsNotNull(property[m_notionType][0]["text"]);
            Assert.IsNotNull(property[m_notionType][0]["text"]["content"]);
            Value = property[m_notionType][0]["text"]["content"].Value<string>();
        }
    }
}
