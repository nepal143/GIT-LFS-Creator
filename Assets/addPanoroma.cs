using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;
using SFB; // Standalone File Browser namespace

public class ImageUploader : MonoBehaviour
{
    public GameObject cameraPrefab;
    public GameObject rawImagePrefab;
    public Transform imageGridContainer;
    public Button uploadButton;
    public Button nextButton;
    public GameObject parent;
    public GameObject invertedSpherePrefab;
    public Transform spawnPoint;
    public float separationDistance = 10f;
    public List<Texture2D> uploadedTextures = new List<Texture2D>();
    public List<GameObject> displayedImages = new List<GameObject>();

    private string username; // Temporary username
    private string propertyName; // Temporary property name

    private const string UPLOAD_URL = "https://theserver-tp6r.onrender.com/upload-image"; // Replace with your server URL

    void Start()
    {
        username = PlayerPrefs.GetString("username", "");
        propertyName = PlayerPrefs.GetString("propertyName", "");
        uploadButton.onClick.AddListener(UploadImage);
        nextButton.onClick.AddListener(() => CreatePanoramaSpheres(parent.transform));
    }

    public void UploadImage()
    {
        var extensions = new[] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg", "bmp", "gif"),
            new ExtensionFilter("All Files", "*"),
        };
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open Image File", "", extensions, false);

        if (paths != null && paths.Length > 0)
        {
            string path = paths[0];
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            DisplayImage(texture);
            uploadedTextures.Add(texture);
        }
        else
        {
            Debug.Log("No file selected.");
        }
    }

    private IEnumerator UploadImageToServer(string path)
    {
        byte[] fileData = File.ReadAllBytes(path);
        string fileName = Path.GetFileName(path);
        string fileType = "image/png"; // Adjust MIME type as needed

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>
    {
        new MultipartFormDataSection("organisationName", username),
        new MultipartFormDataSection("parentPropertyName", propertyName),
        new MultipartFormDataSection("childPropertyName", "defaultChild"),
        new MultipartFormFileSection("file", fileData, fileName, fileType)
    };

        UnityWebRequest www = UnityWebRequest.Post(UPLOAD_URL, formData);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to upload image to server: " + www.error);
            Texture2D texture = new Texture2D(2, 2);
            DisplayImage(texture);
        }
        else
        {
            Debug.Log("Image uploaded successfully: " + www.downloadHandler.text);

            // Load the uploaded image as a texture and display it
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            uploadedTextures.Add(texture);
            DisplayImage(texture);
        }
    }
    public void DisplayImage(Texture2D texture)
    {
        GameObject imageObj = Instantiate(rawImagePrefab, imageGridContainer);
        RawImage rawImage = imageObj.GetComponentInChildren<RawImage>();
        if (rawImage != null)
        {
            rawImage.texture = texture;
            displayedImages.Add(imageObj);
        }
        else
        {
            Debug.LogError("RawImage component not found in children of imageObj.");
        }
    }

    public void DeleteImage(GameObject imageObj)
    {
        Debug.Log("Attempting to delete image object: " + imageObj.name);

        RawImage rawImage = imageObj.GetComponentInChildren<RawImage>();
        if (rawImage != null && rawImage.texture != null)
        {
            int index = uploadedTextures.IndexOf((Texture2D)rawImage.texture);
            if (index != -1)
            {
                uploadedTextures.RemoveAt(index);
                Debug.Log("Texture removed from uploadedTextures list. Count: " + uploadedTextures.Count);
            }
        }

        displayedImages.Remove(imageObj);
        Destroy(imageObj);
        Debug.Log("Image deleted.");
    }

    public void CreatePanoramaSpheres(Transform parent)
    {
        Debug.Log("Creating Panorama Spheres");
        Debug.Log("Number of uploaded textures: " + uploadedTextures.Count);

        GameObject cameraObj = Instantiate(cameraPrefab, spawnPoint.position, Quaternion.identity);
        GameObject spheresParent = new GameObject("SpheresParent");
        spheresParent.transform.parent = parent;

        for (int i = 0; i < uploadedTextures.Count; i++)
        {
            Debug.Log("Creating sphere " + i);
            GameObject sphere = Instantiate(invertedSpherePrefab, spawnPoint.position + Vector3.right * i * separationDistance, Quaternion.identity, spheresParent.transform);

            Renderer sphereRenderer = sphere.GetComponent<Renderer>();
            if (sphereRenderer != null)
            {
                Debug.Log("Assigning texture to sphere " + i);
                Material material = new Material(sphereRenderer.material);
                material.mainTexture = uploadedTextures[i];
                sphereRenderer.material = material;
            }
            else
            {
                Debug.LogError("Sphere Renderer not found on sphere " + i);
            }
        }
    }
}
