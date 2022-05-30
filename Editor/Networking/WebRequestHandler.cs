using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace NotionToUnity.Editor
{
    public static class WebRequestHandler
    {
        private const string NotionAPIRoot = "https://api.notion.com/v1/databases";

        // Use ugly callback pattern here instead of async await due to complications beyond my understanding
        // (tldr there's lots of problems bc this is an editor window)

        public static void GetDatabaseContents(List<NotionDatabaseDefinition> databases,
            string apiToken,
            string apiVersion,
            Action<Dictionary<string, JObject>> onResultCb,
            Action errorCb)
        {
            var mutex = new Mutex();
            var results = new Dictionary<string, JObject>(databases.Count());

            // TODO: turn this into a common function
            Action<string, JObject> onResults = (id, json) => {
                mutex.WaitOne();
                results.Add(id, json);

                if (results.Count == databases.Count)
                {
                    onResultCb.Invoke(results);
                    mutex.ReleaseMutex();
                    try
                    {
                        mutex.Dispose();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    mutex.ReleaseMutex();
                }
            };
            foreach (var db in databases)
            {
                // TODO: combine paginated results
                SendWebRequest($"{NotionAPIRoot}/{{0}}/query", db.Id,
                    "POST", apiToken, apiVersion, onResults, errorCb);
            }
        }

        public static void GetDatabaseProperties(List<NotionDatabaseDefinition> databases,
            string apiToken,
            string apiVersion,
            Action<List<JObject>> onResultCb,
            Action errorCb)
        {
            var mutex = new Mutex();
            var results = new List<JObject>(databases.Count());
            for (int i = 0; i < databases.Count(); i++)
                results.Add(null);

            Action<string, JObject> onResults = (id, json) => {
                mutex.WaitOne();
                int index = databases.FindIndex(definition => definition.Id == id);
                results[index] = json;

                if (results.All(element => element != null))
                {
                    onResultCb.Invoke(results);
                    mutex.ReleaseMutex();
                    try
                    {
                        mutex.Dispose();
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                }
                else
                {
                    mutex.ReleaseMutex();
                }
            };
            foreach (var db in databases)
            {
                SendWebRequest($"{NotionAPIRoot}/{{0}}", db.Id, "GET", apiToken, apiVersion, onResults, errorCb);
            }
        }

        private static void SendWebRequest(string uriFormat,
            string dbId,
            string method,
            string apiToken,
            string apiVersion,
            Action<string, JObject> onResultCb,
            Action errorCb)
        {
            var requestWrapper = new UnityWebRequestWrapper(
                string.Format(uriFormat, dbId), method, apiToken, apiVersion);
            var req = requestWrapper.Request;
            var asyncReq = req.SendWebRequest();

            asyncReq.completed += op => {
                if (!requestWrapper.HandleError())
                {
                    errorCb.Invoke();
                }

                if (!TryParseJson(req.downloadHandler.text, out var json))
                {
                    Debug.LogError("Error parsing json.");
                    errorCb.Invoke();
                }

                onResultCb.Invoke(dbId, json);
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
