using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;

public class SaveToAWS : MonoBehaviour
{
    public ImageUploader imageUploader; // Reference to ImageUploader script
    public string baseUrl = "http://localhost:3000"; // Adjust base URL as needed
    public string folderName = "PanoramaImages"; // Folder name in S3

    private int imageIndex = 1; // Index to keep track of image order

    public void UploadToAWS()
    {
        StartCoroutine(UploadImagesToS3());
        RecordSceneData();
    }

    private IEnumerator UploadImagesToS3()
    {
        foreach (Texture2D texture in imageUploader.uploadedTextures)
        {
            byte[] textureBytes = texture.EncodeToPNG();
            string textureName = $"{imageIndex}.png"; // Naming convention: 1.png, 2.png, 3.png, ...

            WWWForm form = new WWWForm();
            form.AddField("username", imageUploader.username);
            form.AddField("propertyName", imageUploader.propertyName);
            form.AddField("folderName", folderName); // Pass the folder name to server
            form.AddBinaryData("file", textureBytes, textureName, "image/png");

            using (UnityWebRequest request = UnityWebRequest.Post($"{baseUrl}/upload-image", form))
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
        WWWForm form = new WWWForm();
        form.AddField("username", imageUploader.username);
        form.AddField("propertyName", imageUploader.propertyName);

        using (UnityWebRequest request = UnityWebRequest.Post($"{baseUrl}/save-positions-rotations", form))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log($"Sending JSON: {json}"); // Log JSON data being sent
            Debug.Log($"URL: {baseUrl}/save-positions-rotations"); // Log URL being requested

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
