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
    public TMP_Text thumbnailFilePathText; // Container for thumbnail file path
    public TMP_Text filePathsText; // Container for multiple image file paths
    public TMP_Text videoFilePathText; // Container for video file path
    public ParentPropertySaveToAWS saveToAWS; 

    private string thumbnailFilePath;
    private List<string> imageFilePaths = new List<string>();
    private string videoFilePath;

    void Start()
    {
        // Ensure the button references are not null before adding listeners
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
            thumbnailFilePathText.text = "Selected Thumbnail: " + paths[0]; // Display the selected file path
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
            filePathsText.text = "Selected Images:\n" + string.Join("\n", paths);
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
            videoFilePathText.text = "Selected Video: " + paths[0]; // Display the selected video file path
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
}
