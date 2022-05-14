using Newtonsoft.Json.Linq;

// ReSharper disable once CheckNamespace
namespace NotionToUnity
{
    public static class ClassExtensions
    {
        public static string GetKey(this JToken token)
        {
            string path = token.Path;
            if (path[path.Length - 1] == ']')
            {
                int index = path.LastIndexOf('[');
                string key = path.Substring(index);

                // Trim first and last 2 chars ("['key']" -> "key")
                return key.Substring(2, key.Length - 4);
            }
            else
            {
                int index = path.LastIndexOf('.');
                return path.Substring(index + 1);
            }
        }

        public static string RemoveSpaces(this string s)
        {
            return s.Replace(" ", "");
        }
    }
}
