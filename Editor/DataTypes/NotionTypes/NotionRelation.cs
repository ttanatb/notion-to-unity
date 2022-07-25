using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using NotionToUnity.Utils;

namespace NotionToUnity.DataTypes
{
    /// <summary>
    /// Corresponds to a relational database type in Notion.
    /// </summary>
    public class NotionRelation : NotionType<int[]>
    {
        /// <summary>
        /// Type of the corresponding database.
        /// </summary>
        // private Type m_valueType;

        public NotionRelation(JToken property, Type localDbItemType) : base(property, localDbItemType)
        {
            Asserter.AreEqual("relation", m_notionType);

            var relations = property[m_notionType];
            if (!relations.HasValues)
                return;

            int count = relations.Count();
            Value = new int[count];
            using (var hasher = MD5.Create())
            {
                for (int i = 0; i < count; i++)
                {
                    var relation = relations[i];
                    Asserter.IsNotNull(relation);
                    Asserter.IsNotNull(relation["id"]);

                    string id = relation["id"].Value<string>();
                    Asserter.IsNotNull(id);
                    Value[i] = BitConverter.ToInt32(
                        hasher.ComputeHash(Encoding.UTF8.GetBytes(id)), 0);
                }
            }
        }
    }
}
