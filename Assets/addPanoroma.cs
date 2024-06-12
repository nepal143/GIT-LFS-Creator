using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;
using System.IO.Compression; // Added for ZipArchive
using System.Linq; 

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
    public string exportFolderPath = "MyExports";
    public string exportFileName = "ExportedScene.zip";

    private string resourcesImageFolder = "ImportedImages";

    void Start()
    {
        uploadButton.onClick.AddListener(UploadImage);
        nextButton.onClick.AddListener(() => CreatePanoramaSpheres(parent.transform));
    }

    public void UploadImage()
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                string destinationPath = CopyImageToPersistentDataPath(path);
                if (!string.IsNullOrEmpty(destinationPath))
                {
                    StartCoroutine(LoadTextureFromFile(destinationPath));
                }
                else
                {
                    Debug.LogError("Failed to copy image to persistent data path!");
                }
            }
        }, "Select a PNG image");

        Debug.Log("Permission result: " + permission);
    }

    private string CopyImageToPersistentDataPath(string originalPath)
    {
        string destinationFileName = (uploadedTextures.Count + 1).ToString() + ".png";
        string destinationPath = Path.Combine(Application.persistentDataPath, resourcesImageFolder, destinationFileName);

        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, resourcesImageFolder)))
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, resourcesImageFolder));
        }

        try
        {
            File.Copy(originalPath, destinationPath, true);
            return destinationPath;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error copying image to persistent data path: " + ex.Message);
            return null;
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

    public void ExportTextures()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, exportFolderPath);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        foreach (Texture2D texture in uploadedTextures)
        {
            string texturePath = Path.Combine(folderPath, texture.name + ".png");
            byte[] textureBytes = texture.EncodeToPNG();
            File.WriteAllBytes(texturePath, textureBytes);
        }

        Debug.Log("Textures exported successfully.");
    }

    public void ExportAssets()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, exportFolderPath);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string packagePath = Path.Combine(folderPath, exportFileName);

        // Create a zip file
        using (FileStream zipToOpen = new FileStream(packagePath, FileMode.Create))
        {
            using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
            {
                foreach (Texture2D texture in uploadedTextures)
                {
                    string texturePath = Path.Combine(folderPath, texture.name + ".png");
                    archive.CreateEntryFromFile(texturePath, Path.GetFileName(texturePath));
                }
            }
        }

        Debug.Log("Assets exported to " + packagePath);
    }

    public void ExportAll()
    {
        ExportTextures();
        ExportAssets();
    }
}
