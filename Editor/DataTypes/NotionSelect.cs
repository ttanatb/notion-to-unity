using System;
using Newtonsoft.Json.Linq;
using UnityEngine.Assertions;

// ReSharper disable once CheckNamespace
namespace NotionToUnity.Editor
{
    public class NotionSelect : NotionType<int>
    {
        private Type m_valueType;

        public NotionSelect(JToken property, Type dbItemType): base(property, dbItemType)
        {
            Assert.AreEqual("select", m_notionType);

            if (!property[m_notionType].HasValues)
                return;

            Assert.IsNotNull(property[m_notionType]["name"]);
            m_valueType = dbItemType.GetNestedType($"{m_name}Enum");
            Assert.IsNotNull(m_valueType);
            Assert.IsTrue(m_valueType.IsEnum);

            Assert.IsNotNull(property[m_notionType]["name"]);
            string enumString = property[m_notionType]["name"].Value<string>() ?? "Invalid";
            Value = (int)Enum.Parse(m_valueType, enumString.RemoveSpaces());
        }
    }
}
