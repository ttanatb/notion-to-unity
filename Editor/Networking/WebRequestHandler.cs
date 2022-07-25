using System;

namespace NotionToUnity.Networking
{
    /// <summary>
    /// Abstract class for handling web requests for each engine.
    /// </summary>
    public abstract class WebRequestHandler
    {
        /// <summary>
        /// Creates a web request handler specific for the current engine.
        /// </summary>
        public static WebRequestHandler Create(string uri, string method, string apiToken, string apiVersion, string postData)
        {
            #if UNITY_EDITOR
            return new UnityWebRequestHandler(uri, method, apiToken, apiVersion, postData);
            #else            
            throw new NotImplementedException();
            #endif
            
        }
        
        protected WebRequestHandler(string uri, string method, string apiToken, string apiVersion, string postData)
        {

        }
        
        public abstract void Send(string dbId,
            Action<string, string> onResultCb,
            Action errorCb);
    }
}
