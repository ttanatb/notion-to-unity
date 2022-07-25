#if UNITY_EDITOR || UNITY_STANDALONE
#define UNITY
using UnityEngine;
#endif

namespace NotionToUnity.Utils
{
    /// <summary>
    /// Abstract wrapper for logging nonsense.
    /// 
    /// Logger? I barely even know 'er!
    /// </summary>
    public static class Logger
    {
        public static void Log(string message)
        {
        #if UNITY
            Debug.Log(message);
        #endif // UNITY
        }
        
        public static void LogError(string message)
        {
        #if UNITY
            Debug.LogError(message);
        #endif // UNITY
        }
        
        public static void LogWarning(string message)
        {
        #if UNITY
            Debug.LogWarning(message);
        #endif // UNITY
        }
    }
}
