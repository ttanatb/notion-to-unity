using UnityEngine.Networking;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace NotionToUnity
{
    public class UnityWebRequestWrapper
    {
        public UnityWebRequest Request { get; set; }

        public UnityWebRequestWrapper(string uri, string method, string apiToken, string apiVersion)
        {
            Request = new UnityWebRequest(uri, method);
            Request.downloadHandler = new DownloadHandlerBuffer();
            Request.SetRequestHeader("Authorization", $"Bearer {apiToken}");
            Request.SetRequestHeader("Content-Type", "application/json");
            Request.SetRequestHeader("Notion-Version", apiVersion);
        }

        ~UnityWebRequestWrapper()
        {
            Request.Dispose();
        }

        public bool HandleError()
        {
            switch (Request.result)
            {
                case UnityWebRequest.Result.Success:
                    return true;
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError($"<Error while executing ({Request}): {Request.error}>");
                    Debug.LogError(Request.downloadHandler.text);
                    break;
                case UnityWebRequest.Result.InProgress:
                default:
                    Debug.LogError($"Unexpected result: {Request.result}");
                    break;
            }
            return false;
        }
    }
}

