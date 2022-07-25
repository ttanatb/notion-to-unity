using System;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;

namespace NotionToUnity.Networking
{
    /// <summary>
    /// Handles the web request in an ugly callback pattern because Unity Editor is quirky XD
    /// </summary>
    public class UnityWebRequestHandler : WebRequestHandler
    {
        private readonly UnityWebRequest m_request = null;

        public UnityWebRequestHandler(string uri, string method, string apiToken, string apiVersion, string postData) :
            base(uri, method, apiToken, apiVersion, postData)
        {
            m_request = new UnityWebRequest(uri, method);
            m_request.downloadHandler = new DownloadHandlerBuffer();
            m_request.SetRequestHeader("Authorization", $"Bearer {apiToken}");
            m_request.SetRequestHeader("Content-Type", "application/json");
            m_request.SetRequestHeader("Notion-Version", apiVersion);

            if (string.IsNullOrEmpty(postData))
                return;
            m_request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(postData));
        }

        ~UnityWebRequestHandler()
        {
            m_request.Dispose();
        }

        private bool HandleError()
        {
            switch (m_request.result)
            {
                case UnityWebRequest.Result.Success:
                    return true;
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError($"<Error while executing ({m_request}): {m_request.error}>");
                    Debug.LogError(m_request.downloadHandler.text);
                    break;
                case UnityWebRequest.Result.InProgress:
                default:
                    Debug.LogError($"Unexpected result: {m_request.result}");
                    break;
            }
            return false;
        }
        
        /// <summary>
        /// Sends the web request and invokes the respective callback with the result.
        /// </summary>
        /// <param name="dbId">Database ID to associate this request with</param>
        /// <param name="onResultCb">Invoked when results are available, provides database ID and resulting JSON</param>
        /// <param name="errorCb">Invoked on errors</param>
        public override void Send(string dbId,
            Action<string, string> onResultCb,
            Action errorCb)
        {
            var asyncReq = m_request.SendWebRequest();

            asyncReq.completed += op => {
                if (!HandleError())
                {
                    errorCb.Invoke();
                    return;
                }
                
                onResultCb.Invoke(dbId, m_request.downloadHandler.text);
            };
        }
    }
}
