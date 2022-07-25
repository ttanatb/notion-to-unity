#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace NotionToUnity.Utils.Unity
{
    public static class ClassExtensions
    {
        public static ScriptableObject[] GetAllInstances(this string typeName)
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeName);
            var scriptableObjs = new ScriptableObject[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                scriptableObjs[i] = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            }

            return scriptableObjs;

        }
    }
}
#endif
