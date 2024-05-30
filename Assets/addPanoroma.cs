using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ImageUploader : MonoBehaviour
{
    public GameObject cameraPrefab; // Reference to the camera prefab
    public GameObject rawImagePrefab;
    public Transform imageGridContainer;
    public Button uploadButton;
    public Button nextButton;
    // public Text statusText;
    public GameObject invertedSpherePrefab;
    public Transform spawnPoint;
    public float separationDistance = 10f; // Distance between spheres

    public List<Texture2D> uploadedTextures = new List<Texture2D>();
    public List<GameObject> displayedImages = new List<GameObject>();

    void Start()
    {
        uploadButton.onClick.AddListener(UploadImage);
        nextButton.onClick.AddListener(CreatePanoramaSpheres);
    }

    public void UploadImage()
{
    NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
    {
        if (path != null)
        {
            Texture2D texture = NativeGallery.LoadImageAtPath(path, -1);
            if (texture != null)
            {
                DisplayImage(texture);
                uploadedTextures.Add(texture); // Add the texture to the list
                Debug.Log("Texture added to uploadedTextures list. Count: " + uploadedTextures.Count);
            }
            else
            {
                Debug.LogError("Failed to load texture!");
            }
        }
    }, "Select a PNG image");

    Debug.Log("Permission result: " + permission);
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
public void CreatePanoramaSpheres()
{
    Debug.Log("Creating Panorama Spheres");
    Debug.Log("Number of uploaded textures: " + uploadedTextures.Count);

    // Instantiate the camera prefab at the center of the first sphere
    GameObject cameraObj = Instantiate(cameraPrefab, spawnPoint.position, Quaternion.identity);

    for (int i = 0; i < uploadedTextures.Count; i++)
    {
        Debug.Log("Creating sphere " + i);
        
        // Instantiate an inverted sphere at a specific position
        GameObject sphere = Instantiate(invertedSpherePrefab, spawnPoint.position + Vector3.right * i * separationDistance, Quaternion.identity);
        
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

}
