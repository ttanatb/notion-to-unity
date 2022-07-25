using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NotionToUnity.Utils;
using NotionToUnity.DataTypes;

namespace NotionToUnity.Networking
{
    /// <summary>
    /// Interfaces with the Notion API and makes the web request.
    ///
    /// This classes uses an ugly callback pattern because a nicer async/await pattern does not work
    /// with Unity in the editor.
    /// </summary>
    public static class NotionApi
    {
        // TODO: extend versioning to result as well so JSON can be dissected properly.
        public enum Version
        {
            Unsupported = 0,
            Feb2022,
            June2022,
        }

        private static readonly Dictionary<Version, string> VERSION_TO_STRING = new Dictionary<Version, string>()
        {
            { Version.Feb2022, "2022-02-22" },
            { Version.June2022, "2022-06-28" },
        };

        private const string NotionAPIRoot = "https://api.notion.com/v1/databases";

        private static Mutex m_getDbContentsMutex = new Mutex();

        /// <summary>
        /// Queries all of the items from the given database. This can be used to generate the items to propagate in
        /// the local database.
        /// </summary>
        /// <param name="databases">List of databases to query from</param>
        /// <param name="apiToken">API Token</param>
        /// <param name="apiVersion">API Version</param>
        /// <param name="onResultCb">Invoked when all results are processed</param>
        /// <param name="errorCb">Invoked on error</param>
        public static void GetDatabaseContents(List<NotionDatabaseDefinition> databases,
            string apiToken,
            Version apiVersion,
            Action<Dictionary<string, List<JObject>>> onResultCb,
            Action errorCb)
        {
            if (!VERSION_TO_STRING.ContainsKey(apiVersion))
            {
                Logger.LogError($"Invalid API version: {apiVersion}");
                errorCb.Invoke();
                return;
            }

            var results = new Dictionary<string, List<JObject>>(databases.Count());
            var dbToCursor = databases.ToDictionary<NotionDatabaseDefinition, string, string>
                (db => db.Id, db => null);

            void OnSingleReq(string id, string jsonString)
            {
                if (!TryParseJson(jsonString, out var json))
                {
                    Logger.LogError("Error parsing json.");
                    errorCb.Invoke();
                    return;
                }

                m_getDbContentsMutex.WaitOne();

                if (results.ContainsKey(id))
                    results[id].Add(json);
                else
                    results.Add(id, new List<JObject>() { json });

                Asserter.IsNotNull(json["has_more"]);
                if (json["has_more"].Value<bool>())
                {
                    Asserter.IsNotNull(json["next_cursor"]);
                    string nextCursor = json["next_cursor"].Value<string>();
                    dbToCursor[id] = nextCursor;
                    SendReqHelper(id, apiToken, VERSION_TO_STRING[apiVersion], dbToCursor, OnSingleReq, errorCb);
                }
                else
                {
                    dbToCursor.Remove(id);
                }

                // Check if all requests are complete
                if (dbToCursor.Count == 0)
                {
                    m_getDbContentsMutex.ReleaseMutex();
                    onResultCb.Invoke(results);
                    return;
                }

                m_getDbContentsMutex.ReleaseMutex();
            }

            foreach (var db in databases)
                SendReqHelper(db.Id, apiToken, VERSION_TO_STRING[apiVersion], dbToCursor, OnSingleReq, errorCb);
        }

        private static void SendReqHelper(string dbId, string apiToken, string apiVersion, 
            IReadOnlyDictionary<string, string> dbToCursor, Action<string, string> onResultCb, Action errorCb)
        {
            string postData = string.IsNullOrEmpty(dbToCursor[dbId]) ? null : new JObject(
                new JProperty("start_cursor", dbToCursor[dbId])
            ).ToString(Formatting.None);
            
            var request = WebRequestHandler.Create(
                $"{NotionAPIRoot}/{dbId}/query", "POST", apiToken, apiVersion, postData);
            request.Send(dbId, onResultCb, errorCb);
        }

        /// <summary>
        /// Gets the properties associated with the given database. This can be used to generate code for local schema.
        /// </summary>
        /// <param name="databases">List of databases to query property of</param>
        /// <param name="apiToken">API Token</param>
        /// <param name="apiVersion">API Version</param>
        /// <param name="onResultCb">Invoked when all results are processed</param>
        /// <param name="errorCb">Invoked on error</param>
        public static void GetDbProperties(List<NotionDatabaseDefinition> databases,
            string apiToken,
            Version apiVersion,
            Action<Dictionary<string, JObject>> onResultCb,
            Action errorCb)
        {
            if (!VERSION_TO_STRING.ContainsKey(apiVersion))
            {
                Logger.LogError($"Invalid API version: {apiVersion}");
                errorCb.Invoke();
                return;
            }
            
            var mutex = new Mutex();
            var results = new Dictionary<string, JObject>(databases.Count());

            foreach (var db in databases)
            {
                var request = WebRequestHandler.Create(
                    $"{NotionAPIRoot}/{db.Id}", "GET", apiToken, VERSION_TO_STRING[apiVersion], null);
                request.Send(db.Id, OnSingleResultCb(mutex, results, databases.Count, onResultCb, errorCb), errorCb);
            }
        }

        /// <summary>
        /// Helper function used to create callback to insert a result for one database to the collection of results.
        /// </summary>
        /// <param name="mutex">Mutex used block access to collection</param>
        /// <param name="results">Collection to insert final results to</param>
        /// <param name="reqCount">Number of rqeuests to wait on</param>
        /// <param name="onResultCb">Invoked when all results are processed</param>
        /// <param name="errorCb">Invoked on errors</param>
        /// <param name="mergeCb">Used to merge</param>
        /// <returns></returns>
        private static Action<string, string> OnSingleResultCb<T>(Mutex mutex,
            T results,
            int reqCount,
            Action<T> onResultCb,
            Action errorCb,
            Action<T, string, JObject> mergeCb = null) where T : IDictionary
        {
            return (id, jsonString) => {
                if (!TryParseJson(jsonString, out var json))
                {
                    Logger.LogError("Error parsing json.");
                    errorCb.Invoke();
                    return;
                }

                mutex.WaitOne();
                
                // For when request count does not match database count (querying a database can return paginated
                // results, so you gotta split database query into multiple requests).
                if (results.Contains(id))
                {
                    if (mergeCb == null)
                    {
                        Logger.LogError("Unsupported merge function (there should only be one db for req)");
                        errorCb.Invoke();
                        return;
                    }
                    mergeCb(results, id, json);
                }
                else
                {
                    results.Add(id, json);
                }

                // Check if all requests are complete
                if (results.Count == reqCount)
                {
                    mutex.ReleaseMutex();
                    try
                    {
                        mutex.Dispose();
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"Error disposing mutex: {e}.");
                    }
                    onResultCb.Invoke(results);
                }
                else
                {
                    mutex.ReleaseMutex();
                }
            };
        }

        /// <summary>
        /// Parses JSON using Newtonsoft JSON
        /// </summary>
        private static bool TryParseJson(string json, out JObject jObject)
        {
            jObject = null;
            try
            {
                jObject = JObject.Parse(json);
            }
            catch (Exception e)
            {
                Logger.LogError($"Error parsing JSON: {e}.");
                return false;
            }
            return true;
        }
    }
}
