#if UNITY_EDITOR 
using UnityEditor;
using UnityEngine;
#endif

namespace NotionToUnity.Utils
{
    public static class GenericSave
    {
        public static void SaveSo(object so)
        {
            #if UNITY_EDITOR
            EditorUtility.SetDirty((ScriptableObject)so);
            Undo.RecordObject((ScriptableObject)so, "Updated data");
            AssetDatabase.SaveAssets();
            #endif
        }
    }
}
