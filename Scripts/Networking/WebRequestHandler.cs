using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace NotionToUnity
{
    public static class WebRequestHandler
    {
        private const string NotionAPIRoot = "https://api.notion.com/v1/databases";

        // Use ugly callback pattern here instead of async await due to complications beyond my understanding
        // (tldr there's lots of problems bc this is an editor window)

        public static void QueryDatabase(string databaseId,
            string apiToken,
            string apiVersion,
            Action<JObject> completionCb,
            Action errorCb)
        {
            // TODO: combine paginated results
            SendWebRequest($"{NotionAPIRoot}/{databaseId}/query", "POST",
                apiToken, apiVersion, completionCb, errorCb);
        }

        public static void GetDatabaseProperties(string databaseId,
            string apiToken,
            string apiVersion,
            Action<JObject> completionCb,
            Action errorCb)
        {
            SendWebRequest($"{NotionAPIRoot}/{databaseId}", "GET", apiToken, apiVersion, completionCb,
                errorCb);
        }

        private static void SendWebRequest(string uri,
            string method,
            string apiToken,
            string apiVersion,
            Action<JObject> completionCb,
            Action errorCb)
        {
            var requestWrapper = new UnityWebRequestWrapper(uri, method, apiToken, apiVersion);
            var req = requestWrapper.Request;
            req.SendWebRequest().completed += o => {
                if (!requestWrapper.HandleError())
                {
                    errorCb.Invoke();
                    return;
                }

                if (!TryParseJson(req.downloadHandler.text, out var json))
                {
                    Debug.LogError("Error parsing json.");
                    errorCb.Invoke();
                    return;
                }

                completionCb.Invoke(json);
            };
        }

        private static bool TryParseJson(string json, out JObject jObject)
        {
            jObject = null;
            try
            {
                jObject = JObject.Parse(json);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
            return true;
        }
    }
}
