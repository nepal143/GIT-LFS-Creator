using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SFB; 
using TMPro; 

public class AWSImageUploader : MonoBehaviour
{
    public Button uploadThumbnailButton;
    public Button uploadMultipleImagesButton;
    public Button uploadVideoButton;
    public Button saveButton;

    public GameObject thumbnailContainer; // Container for single image preview
    public GameObject multipleImagesContainer; // Container for multiple image previews
    public GameObject imagePrefab; // Prefab with a RawImage component
    public ParentPropertySaveToAWS saveToAWS;

    private string thumbnailFilePath;
    private List<string> imageFilePaths = new List<string>();
    private string videoFilePath;

    // Maximum dimensions for the image preview
    private const float maxPreviewWidth = 100f;
    private const float maxPreviewHeight = 100f;

    void Start()
    {
        if (uploadThumbnailButton != null)
        {
            uploadThumbnailButton.onClick.AddListener(OpenThumbnailFileBrowser);
        }

        if (uploadMultipleImagesButton != null)
        {
            uploadMultipleImagesButton.onClick.AddListener(OpenMultipleImagesFileBrowser);
        }

        if (uploadVideoButton != null)
        {
            uploadVideoButton.onClick.AddListener(OpenVideoFileBrowser);
        }

        if (saveButton != null)
        {
            saveButton.onClick.AddListener(SaveAssetsToAWS);
        }
    }

    private void OpenThumbnailFileBrowser()
    {
        var extensions = new[] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg", "bmp", "gif"),
            new ExtensionFilter("All Files", "*"),
        };
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open Thumbnail Image", "", extensions, false);

        if (paths != null && paths.Length > 0)
        {
            thumbnailFilePath = paths[0];
            StartCoroutine(LoadAndDisplayImage(paths[0], thumbnailContainer));
        }
        else
        {
            Debug.Log("No file selected.");
        }
    }

    private void OpenMultipleImagesFileBrowser()
    {
        var extensions = new[] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg", "bmp", "gif"),
            new ExtensionFilter("All Files", "*"),
        };
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open Image Files", "", extensions, true);

        if (paths != null && paths.Length > 0)
        {
            imageFilePaths.Clear();
            imageFilePaths.AddRange(paths);
            foreach (string path in paths)
            {
                StartCoroutine(LoadAndDisplayImage(path, multipleImagesContainer));
            }
        }
        else
        {
            Debug.Log("No files selected.");
        }
    }

    private void OpenVideoFileBrowser()
    {
        var extensions = new[] {
            new ExtensionFilter("Video Files", "mp4", "mov", "wmv", "avi", "mkv"),
            new ExtensionFilter("All Files", "*"),
        };
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open Video File", "", extensions, false);

        if (paths != null && paths.Length > 0)
        {
            videoFilePath = paths[0];
        }
        else
        {
            Debug.Log("No file selected.");
        }
    }

    private void SaveAssetsToAWS()
    {
        if (!string.IsNullOrEmpty(thumbnailFilePath))
        {
            saveToAWS.UploadSingleFileToAWS(thumbnailFilePath, "thumbnails");
        }

        if (imageFilePaths.Count > 0)
        {
            saveToAWS.UploadMultipleFilesToAWS(imageFilePaths, "images");
        }

        if (!string.IsNullOrEmpty(videoFilePath))
        {
            saveToAWS.UploadSingleFileToAWS(videoFilePath, "videos");
        }
    }

    private IEnumerator LoadAndDisplayImage(string path, GameObject container)
    {
        // Load the image from file path
        byte[] fileData = System.IO.File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData); // This will auto-resize the texture dimensions

        // Instantiate a new image prefab and set the texture
        GameObject newImage = Instantiate(imagePrefab, container.transform);
        RawImage rawImage = newImage.GetComponent<RawImage>();
        rawImage.texture = texture;

        // Resize the image to fit within the specified max width and height
        float aspectRatio = (float)texture.width / texture.height;
        float width = texture.width;
        float height = texture.height;

        if (width > maxPreviewWidth || height > maxPreviewHeight)
        {
            if (width / maxPreviewWidth > height / maxPreviewHeight)
            {
                width = maxPreviewWidth;
                height = width / aspectRatio;
            }
            else
            {
                height = maxPreviewHeight;
                width = height * aspectRatio;
            }
        }

        rawImage.rectTransform.sizeDelta = new Vector2(width, height);
        yield return null;
    }
}
