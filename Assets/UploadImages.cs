using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SFB; // Standalone File Browser namespace
using TMPro; // TextMeshPro namespace

public class AWSImageUploader : MonoBehaviour
{
    public Button uploadButton;
    public Button uploadMultipleButton; // New button for uploading multiple images
    public Button saveButton;
    public TMP_Text filePathsText; // Text component to display file paths
    public SaveToAWS saveToAWS; // Reference to SaveToAWS script

    [SerializeField]
    private string folderName = "Thumbnail"; // Folder name to be set in the Inspector

    private List<Texture2D> uploadedTextures = new List<Texture2D>(); // List to hold multiple textures
    private List<string> uploadedFilePaths = new List<string>(); // List to hold multiple file paths

    void Start()
    {
        uploadButton.onClick.AddListener(OpenFileBrowser);
        uploadMultipleButton.onClick.AddListener(OpenMultipleFileBrowser); // Add listener for new button
        saveButton.onClick.AddListener(SaveImagesToAWS);
    }

    private void OpenFileBrowser()
    {
        var extensions = new[] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg", "bmp", "gif"),
            new ExtensionFilter("All Files", "*"),
        };
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open Image File", "", extensions, false);

        if (paths != null && paths.Length > 0)
        {
            uploadedFilePaths.Clear();
            uploadedFilePaths.Add(paths[0]);
            filePathsText.text = "Selected File: " + paths[0]; // Display the selected file path
            StartCoroutine(LoadTexturesFromFiles(paths));
        }
        else
        {
            Debug.Log("No file selected.");
        }
    }

    private void OpenMultipleFileBrowser()
    {
        var extensions = new[] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg", "bmp", "gif"),
            new ExtensionFilter("All Files", "*"),
        };
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open Image Files", "", extensions, true);

        if (paths != null && paths.Length > 0)
        {
            uploadedFilePaths.Clear();
            uploadedFilePaths.AddRange(paths); 
            filePathsText.text = "Selected Files:\n" + string.Join("\n", paths); 
            StartCoroutine(LoadTexturesFromFiles(paths));
        }
        else
        {
            Debug.Log("No files selected.");
        }
    }

    private IEnumerator LoadTexturesFromFiles(string[] filePaths)
    {
        uploadedTextures.Clear();

        foreach (string filePath in filePaths)
        {
            byte[] fileData = System.IO.File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            uploadedTextures.Add(texture);

            yield return null;
        }
    }

    private void SaveImagesToAWS()
    {
        if (uploadedTextures.Count == 1)
        {
            saveToAWS.UploadSingleImageToAWS(uploadedTextures[0], folderName);
        }
        else if (uploadedTextures.Count > 0)
        {
            saveToAWS.UploadMultipleImagesToAWS(uploadedTextures, folderName);
        }
        else
        {
            Debug.LogError("No textures to upload.");
        }
    }
}
