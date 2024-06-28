using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class ObjAPIManager : MonoBehaviour
{
    private string baseUrl = "http://localhost:3000/user"; // Adjust base URL as needed

    public void UploadObjModel(byte[] modelData, Action<string> callback)
    {
        StartCoroutine(UploadObjModelCoroutine(modelData, callback));
    }

    private IEnumerator UploadObjModelCoroutine(byte[] modelData, Action<string> callback)
    {
        using (UnityWebRequest request = new UnityWebRequest($"{baseUrl}/upload-obj", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(modelData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/octet-stream");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                callback(request.error);
            }
            else
            {
                Debug.Log("OBJ upload complete!");
                string responseText = request.downloadHandler.text;
                callback(responseText);
            }
        }
    }
}
