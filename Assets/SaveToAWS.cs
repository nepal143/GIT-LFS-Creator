using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class SaveToAWS : MonoBehaviour
{
    public string baseUrl = "http://localhost:3000/"; // Adjust base URL as needed
    [SerializeField]
    private string folderName = "PanoramaImages"; // Default folder name in S3
    public ImageUploader imageUploader;
    private string username;
    private string propertyName;
    private int imageIndex = 1;

    void Start()
    {
        username = PlayerPrefs.GetString("username", "");
        propertyName = PlayerPrefs.GetString("propertyName", "");
        Debug.Log(username + " " + propertyName);
    }
    public void UploadToAWS()
    {
        StartCoroutine(UploadImagesToS3Panaroma());
        RecordSceneData();
    }
    private IEnumerator UploadImagesToS3Panaroma()
    {
        foreach (Texture2D texture in imageUploader.uploadedTextures)
        {
            byte[] textureBytes = texture.EncodeToPNG();
            string textureName = $"{imageIndex}.png"; // Naming convention: 1.png, 2.png, 3.png, ...

            WWWForm form = new WWWForm();
            form.AddField("organisationName", PlayerPrefs.GetString("organisationName"));
            form.AddField("parentPropertyName", PlayerPrefs.GetString("parentPropertyName"));
            form.AddField("childPropertyName", PlayerPrefs.GetString("childPropertyName"));
            form.AddBinaryData("file", textureBytes, textureName, "image/png");

            string fullUrl = $"http://localhost:3000/upload-image-panaroma";
            Debug.Log("Uploading to URL: " + fullUrl);

            using (UnityWebRequest request = UnityWebRequest.Post(fullUrl, form))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Error uploading texture {textureName}: {request.error}");
                }
                else
                {
                    Debug.Log($"Texture {textureName} uploaded successfully.");
                    Debug.Log("Response: " + request.downloadHandler.text);
                    imageIndex++; // Increment index for the next image
                }
            }
        }
         SceneManager.LoadScene("parentDashboard");
    }
    public void UploadSingleImageToAWS(Texture2D texture, string folderName)
    {
        StartCoroutine(UploadImageToS3(texture, folderName));
    }

    public void UploadMultipleImagesToAWS(List<Texture2D> textures, string folderName)
    {
        StartCoroutine(UploadImagesToS3(textures, folderName));
    }

    private IEnumerator UploadImageToS3(Texture2D texture, string folderName)
    {
        byte[] textureBytes = texture.EncodeToPNG();
        string textureName = GenerateUniqueFileName("Thumbnail.png"); // Generate unique name for single image

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("propertyName", PlayerPrefs.GetString("parentPropertyName"));
        form.AddField("folderName", folderName); // Pass the folder name to server
        form.AddField("organisationName", PlayerPrefs.GetString("organisationName"));
        form.AddBinaryData("file", textureBytes, textureName, "image/png");

        using (UnityWebRequest request = UnityWebRequest.Post($"{baseUrl}upload/upload-image", form))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error uploading texture {textureName}: {request.error}");
            }
            else
            {
                Debug.Log("Texture uploaded successfully.");
                Debug.Log("Response: " + request.downloadHandler.text);
            }
        }
    }

    private IEnumerator UploadImagesToS3(List<Texture2D> textures, string folderName)
    {
        for (int i = 0; i < textures.Count; i++)
        {
            Texture2D texture = textures[i];
            byte[] textureBytes = texture.EncodeToPNG();
            string textureName = GenerateUniqueFileName($"Image_{i + 1}.png"); // Generate unique name for each image

            WWWForm form = new WWWForm();
            form.AddField("username", username);
            form.AddField("propertyName", PlayerPrefs.GetString("parentPropertyName"));
            form.AddField("folderName", folderName); // Pass the folder name to server
            form.AddField("organisationName", PlayerPrefs.GetString("organisationName"));
            form.AddBinaryData("file", textureBytes, textureName, "image/png");

            using (UnityWebRequest request = UnityWebRequest.Post($"{baseUrl}upload/upload-image", form))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Error uploading texture {textureName}: {request.error}");
                }
                else
                {
                    Debug.Log($"Texture {textureName} uploaded successfully.");
                    Debug.Log("Response: " + request.downloadHandler.text);
                }
            }
        }
    }

    private string GenerateUniqueFileName(string baseName)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        string uniqueIdentifier = Guid.NewGuid().ToString().Substring(0, 8);
        string extension = System.IO.Path.GetExtension(baseName);
        string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(baseName);
        return $"{fileNameWithoutExtension}_{timestamp}_{uniqueIdentifier}{extension}";
    }

    [System.Serializable]
    public class SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }
    }

    [System.Serializable]
    public class SerializableQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public SerializableQuaternion(Quaternion quaternion)
        {
            x = quaternion.x;
            y = quaternion.y;
            z = quaternion.z;
            w = quaternion.w;
        }
    }

    [System.Serializable]
    public class HotspotData
    {
        public SerializableVector3 position;
        public SerializableQuaternion rotation;
        public int connectedPanoramaIndex;
    }

    [System.Serializable]
    public class DollHouseData
    {
        public SerializableVector3 position;
        public SerializableQuaternion rotation;
    }

    [System.Serializable]
    public class SceneData
    {
        public List<HotspotData> hotspots = new List<HotspotData>();
        public DollHouseData dollHouse;
    }

    public void RecordSceneData()
    {
        SceneData sceneData = new SceneData();

        // Find and record DollHouse data
        GameObject dollHouseObject = GameObject.FindWithTag("DollHouse");
        if (dollHouseObject != null)
        {
            sceneData.dollHouse = new DollHouseData
            {
                position = new SerializableVector3(dollHouseObject.transform.position),
                rotation = new SerializableQuaternion(dollHouseObject.transform.rotation)
            };
        }

        // Find and record Hotspot data
        GameObject[] hotspotObjects = GameObject.FindGameObjectsWithTag("Hotspot");
        foreach (GameObject hotspot in hotspotObjects)
        {
            HotspotScript hotspotScript = hotspot.GetComponent<HotspotScript>();
            if (hotspotScript != null)
            {
                HotspotData hotspotData = new HotspotData
                {
                    position = new SerializableVector3(hotspot.transform.position),
                    rotation = new SerializableQuaternion(hotspot.transform.rotation),
                    connectedPanoramaIndex = hotspotScript.connectedPanoramaIndex
                };

                sceneData.hotspots.Add(hotspotData);
            }
        }

        // Convert scene data to JSON
        string json = JsonConvert.SerializeObject(sceneData, Formatting.Indented);
        StartCoroutine(SaveSceneDataToS3(json));
    }

    private IEnumerator SaveSceneDataToS3(string json)
    {
        // Ensure username and propertyName are not null or empty
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(propertyName))
        {
            Debug.LogError("Username or PropertyName is null or empty. Aborting scene data save.");
            yield break; // Exit coroutine early
        }

        WWWForm form = new WWWForm();
       form.AddField("organisationName", PlayerPrefs.GetString("organisationName"));
            form.AddField("parentPropertyName", PlayerPrefs.GetString("parentPropertyName"));
            form.AddField("childPropertyName", PlayerPrefs.GetString("childPropertyName"));
        form.AddField("hotspots", json); // Send scene data JSON as 'hotspots'

        using (UnityWebRequest request = UnityWebRequest.Post($"http://localhost:3000/save-positions-rotations", form))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error saving scene data: {request.error}");
            }
            else
            {
                Debug.Log("Scene data saved successfully.");
                Debug.Log("Response: " + request.downloadHandler.text);
            }
        }
    }
}
