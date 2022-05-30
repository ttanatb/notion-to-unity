using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine.Assertions;

// ReSharper disable once CheckNamespace
namespace NotionToUnity.Editor
{
    public class NotionRelation : NotionType<int[]>
    {
        private Type m_valueType;

        public NotionRelation(JToken property, Type dbItemType) : base(property, dbItemType)
        {
            Assert.AreEqual("relation", m_notionType);

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
                    Assert.IsNotNull(relation);
                    Assert.IsNotNull(relation["id"]);

                    string id = relation["id"].Value<string>();
                    Value[i] = BitConverter.ToInt32(
                        hasher.ComputeHash(Encoding.UTF8.GetBytes(id)), 0);
                }
            }
        }


        public void ResolveType() // TODO: Add params
        {
            // TODO: figure out value type?
        }
    }
}
