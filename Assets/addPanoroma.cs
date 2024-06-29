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
    public SaveToAWS saveToAWS; // Reference to SaveToAWS script
    
    public string username = "tempUser"; // Temporary username
    public string propertyName = "tempProperty"; // Temporary property name
    void Start()
    {
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
            StartCoroutine(LoadTextureFromFile(path));
        }
        else
        {
            Debug.Log("No file selected.");
        }
    }

    private IEnumerator LoadTextureFromFile(string path)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture("file://" + path))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load texture from file: " + uwr.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                if (texture != null)
                {
                    DisplayImage(texture);
                    uploadedTextures.Add(texture);
                    Debug.Log("Texture added to uploadedTextures list. Count: " + uploadedTextures.Count);

                    // Trigger AWS upload via SaveToAWS script
                    // saveToAWS.UploadToAWS();
                }
                else
                {
                    Debug.LogError("Failed to create texture from file!");
                }
            }
        }
    }

    public void DisplayImage(Texture2D texture)
    {
        GameObject imageObj = Instantiate(rawImagePrefab, imageGridContainer);
        RawImage rawImage = imageObj.GetComponentInChildren<RawImage>();
        if (rawImage != null)
        {
            rawImage.texture = texture;
            rawImage.transform.SetParent(imageObj.transform.GetChild(0));
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
