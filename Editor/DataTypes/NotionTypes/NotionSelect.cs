using System;
using Newtonsoft.Json.Linq;
using NotionToUnity.Utils;

namespace NotionToUnity.DataTypes
{
    /// <summary>
    /// Corresponds to the Select type in Notion, which acts like a singular enum.
    /// </summary>
    public class NotionSelect : NotionType<int>
    {
        public NotionSelect(JToken property, Type localDbItemType): base(property, localDbItemType)
        {
            Asserter.AreEqual("select", m_notionType);

            if (!property[m_notionType].HasValues)
                return;

            Asserter.IsNotNull(property[m_notionType]["name"]);
            var valueType = localDbItemType.GetNestedType($"{m_name}Enum");
            Asserter.IsNotNull(valueType);
            Asserter.IsTrue(valueType.IsEnum);

            Asserter.IsNotNull(property[m_notionType]["name"]);
            string enumString = property[m_notionType]["name"].Value<string>() ?? "Invalid";
            Value = (int)Enum.Parse(valueType, enumString.RemoveSpaces());
        }
    }
}
