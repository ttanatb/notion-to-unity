using System.IO;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace NotionToUnity.Utils
{
    public static class GenericPath
    {
        private static string BasePath()
        {
            #if UNITY_EDITOR
            return Application.dataPath;
            #else
            throw new NotImplementedException();
            #endif
        }
        
        public static string ScriptExtension()
        {
            #if UNITY_EDITOR
            return ".cs";
            #else
            throw new NotImplementedException();
            #endif
        }

        public static StreamWriter GetStreamWriter(string relativePath, string fileName, string extension)
        {
            string totalPath = BasePath();
            foreach (string dir in relativePath.Split('/'))
            {
                totalPath = Path.Combine(totalPath, dir);
                if (!Directory.Exists(totalPath))
                    Directory.CreateDirectory(totalPath);
            }
            totalPath = Path.Combine(totalPath, $"{fileName}{extension}");

            return File.Exists(totalPath) ? new StreamWriter(totalPath, false) : File.CreateText(totalPath);
        }
    }
}
