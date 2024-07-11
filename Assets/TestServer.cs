using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TestServerConnection : MonoBehaviour
{
    private const string serverUrl = "https://theserver-tp6r.onrender.com/organisation/organisation/TechCorp"; // Ensure HTTPS

    void Start()
    {
        StartCoroutine(CheckConnection());
    } 

    IEnumerator CheckConnection()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(serverUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                Debug.Log("Testing connection to: " + serverUrl);
                Debug.Log(webRequest.downloadHandler.text);
            }
        }
    }
}
