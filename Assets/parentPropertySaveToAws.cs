using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ParentPropertySaveToAWS : MonoBehaviour
{
    public string baseUrl = "https://theserver-tp6r.onrender.com/"; // Adjust base URL as needed
    private string username;
    private string propertyName;

    void Start()
    {
        username = PlayerPrefs.GetString("username", "");
        propertyName = PlayerPrefs.GetString("propertyName", "");
        Debug.Log(username + " " + propertyName);
    }

    public void UploadSingleFileToAWS(string filePath, string folderName)
    {
        StartCoroutine(UploadFileToS3(filePath, folderName));
    }

    public void UploadMultipleFilesToAWS(List<string> filePaths, string folderName)
    {
        StartCoroutine(UploadFilesToS3(filePaths, folderName));
    }

    private IEnumerator UploadFileToS3(string filePath, string folderName)
    {
        byte[] fileBytes = File.ReadAllBytes(filePath);
        string fileName = GenerateUniqueFileName(Path.GetFileName(filePath));

        WWWForm form = new WWWForm();
        // form.AddField("username", username);
        form.AddField("parentPropertyName", PlayerPrefs.GetString("parentPropertyName"));
        form.AddField("childPropertyName" , PlayerPrefs.GetString("childPropertyName"));
        form.AddField("folderName", folderName);
        form.AddField("organisationName", PlayerPrefs.GetString("organisationName"));
        form.AddBinaryData("file", fileBytes, fileName, "application/octet-stream");

        using (UnityWebRequest request = UnityWebRequest.Post($"https://theserver-tp6r.onrender.com/upload/upload-file/images", form))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error uploading file {fileName}: {request.error}");
            }
            else
            {
                Debug.Log("File uploaded successfully.");
                Debug.Log("Response: " + request.downloadHandler.text);
            }
        }
    }

    private IEnumerator UploadFilesToS3(List<string> filePaths, string folderName)
    {
        foreach (string filePath in filePaths)
        {
            yield return UploadFileToS3(filePath, folderName);
        }
    }

    private string GenerateUniqueFileName(string baseName)
    {
        string timestamp = System.DateTime.Now.ToString("yyyyMMddHHmmssfff");
        string uniqueIdentifier = System.Guid.NewGuid().ToString().Substring(0, 8);
        string extension = Path.GetExtension(baseName);
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(baseName);
        return $"{fileNameWithoutExtension}_{timestamp}_{uniqueIdentifier}{extension}";
    }
}
    