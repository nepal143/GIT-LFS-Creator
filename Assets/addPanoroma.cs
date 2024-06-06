using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class ImageUploader : MonoBehaviour
{
    public GameObject cameraPrefab; // Reference to the camera prefab
    public GameObject rawImagePrefab;
    public Transform imageGridContainer;
    public Button uploadButton;
    public Button nextButton;
    public GameObject parent;
    public GameObject invertedSpherePrefab;
    public Transform spawnPoint;
    public float separationDistance = 10f; // Distance between spheres

    public List<Texture2D> uploadedTextures = new List<Texture2D>();
    public List<GameObject> displayedImages = new List<GameObject>();
    public string exportFolderPath = "Assets/MyExports";
    public string exportFileName = "ExportedScene.unitypackage";

    private string assetsImageFolder = "Assets/ImportedImages";

    void Start()
    {
        uploadButton.onClick.AddListener(UploadImage);
        nextButton.onClick.AddListener(() => CreatePanoramaSpheres(parent.transform)); // Pass parent's transform
    }

    public void UploadImage()
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                string destinationPath = CopyImageToAssets(path);
                if (!string.IsNullOrEmpty(destinationPath))
                {
                    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(destinationPath);
                    if (texture != null)
                    {
                        DisplayImage(texture);
                        uploadedTextures.Add(texture); // Add the texture to the list
                        Debug.Log("Texture added to uploadedTextures list. Count: " + uploadedTextures.Count);
                    }
                    else
                    {
                        Debug.LogError("Failed to load texture from Assets folder!");
                    }
                }
                else
                {
                    Debug.LogError("Failed to copy image to Assets folder!");
                }
            }
        }, "Select a PNG image");

        Debug.Log("Permission result: " + permission);
    }

    private string CopyImageToAssets(string originalPath)
    {
        string fileName = Path.GetFileName(originalPath);
        string destinationPath = Path.Combine(assetsImageFolder, fileName);

        if (!Directory.Exists(assetsImageFolder))
        {
            Directory.CreateDirectory(assetsImageFolder);
            AssetDatabase.Refresh();
        }

        try
        {
            File.Copy(originalPath, destinationPath, true);
            AssetDatabase.ImportAsset(destinationPath);
            return destinationPath;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error copying image to Assets folder: " + ex.Message);
            return null;
        }
    }

    public void DisplayImage(Texture2D texture)
    {
        GameObject imageObj = Instantiate(rawImagePrefab, imageGridContainer);
        RawImage rawImage = imageObj.GetComponentInChildren<RawImage>(); // Get RawImage from children
        if (rawImage != null)
        {
            rawImage.texture = texture; // Assign texture to the RawImage

            // Make the RawImage a child of the DeleteButton
            rawImage.transform.SetParent(imageObj.transform.GetChild(0)); // Assuming the RawImage is the first child of imageObj

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

        // Find the RawImage component in the imageObj
        RawImage rawImage = imageObj.GetComponentInChildren<RawImage>();
        if (rawImage != null && rawImage.texture != null)
        {
            // Find the index of the texture in uploadedTextures
            int index = uploadedTextures.IndexOf((Texture2D)rawImage.texture);
            if (index != -1)
            {
                // Remove the texture from uploadedTextures
                uploadedTextures.RemoveAt(index);
                Debug.Log("Texture removed from uploadedTextures list. Count: " + uploadedTextures.Count);
            }
        }

        // Remove the imageObj from the displayedImages list
        displayedImages.Remove(imageObj);

        // Destroy the imageObj (which includes the RawImage)
        Destroy(imageObj);

        Debug.Log("Image deleted.");
    }

    public void CreatePanoramaSpheres(Transform parent)
    {
        Debug.Log("Creating Panorama Spheres");
        Debug.Log("Number of uploaded textures: " + uploadedTextures.Count);

        // Instantiate the camera prefab at the center of the first sphere
        GameObject cameraObj = Instantiate(cameraPrefab, spawnPoint.position, Quaternion.identity);

        // Create a new GameObject to serve as the parent of the spheres
        GameObject spheresParent = new GameObject("SpheresParent");
        spheresParent.transform.parent = parent; // Set the parent object

        for (int i = 0; i < uploadedTextures.Count; i++)
        {
            Debug.Log("Creating sphere " + i);

            // Instantiate an inverted sphere at a specific position
            GameObject sphere = Instantiate(invertedSpherePrefab, spawnPoint.position + Vector3.right * i * separationDistance, Quaternion.identity, spheresParent.transform);

            Renderer sphereRenderer = sphere.GetComponent<Renderer>();
            if (sphereRenderer != null)
            {
                Debug.Log("Assigning texture to sphere " + i);
                // Create a new material based on the sphere's current material
                Material material = new Material(sphereRenderer.material);

                // Set the main texture of the material to the corresponding uploaded texture
                material.mainTexture = uploadedTextures[i];

                // Assign the modified material back to the sphere's renderer
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
        if (!AssetDatabase.IsValidFolder(exportFolderPath))
        {
            AssetDatabase.CreateFolder("Assets", "MyExports");
        }

        // Export each uploaded texture
        foreach (Texture2D texture in uploadedTextures)
        {
            string texturePath = Path.Combine(exportFolderPath, texture.name + ".png");
            byte[] textureBytes = texture.EncodeToPNG();
            File.WriteAllBytes(texturePath, textureBytes);
        }

        Debug.Log("Textures exported successfully.");
    }

    public void ExportAssets()
    {
        // Export the prefab to a package
        string packagePath = Path.Combine(exportFolderPath, exportFileName);
        AssetDatabase.ExportPackage(AssetDatabase.GetAllAssetPaths(), packagePath, ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Recurse);

        Debug.Log("Root object and its dependencies exported to " + packagePath);
    }

    public void ExportAll()
    {
        ExportTextures();
        ExportAssets();
    }
}
