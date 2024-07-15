using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class ImageUploadApiManager : MonoBehaviour
{
    public string serverUrl = "http://localhost:3000/upload-image";

    public void UploadImage(string username, string propertyName, string folderName, string filePath, System.Action<string> callback)
    {
        StartCoroutine(UploadToServer(username, propertyName, folderName, filePath, callback));
    }

    private IEnumerator UploadToServer(string username, string propertyName, string folderName, string filePath, System.Action<string> callback)
    {
        // Read the file data
        byte[] fileData = File.ReadAllBytes(filePath);
        string fileName = Path.GetFileName(filePath);

        // Create form data
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("propertyName", propertyName);
        form.AddField("folderName", folderName);
        form.AddBinaryData("file", fileData, fileName, "image/png");

        // Send HTTP POST request
        using (UnityWebRequest www = UnityWebRequest.Post(serverUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to upload file: {www.error}");
                callback?.Invoke($"Failed to upload file: {www.error}");
            }
            else
            {
                Debug.Log($"File upload successful: {www.downloadHandler.text}");
                callback?.Invoke(www.downloadHandler.text);
            }
        }
    }
}
