using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using SFB; // Standalone File Browser namespace

public class AWSImageUploader : MonoBehaviour
{
    public Button uploadButton;
    public Button saveButton;
    public SaveToAWS saveToAWS; // Reference to SaveToAWS script

    [SerializeField]
    private string folderName = "Thumbnail"; // Folder name to be set in the Inspector

    private Texture2D uploadedTexture;
    private string uploadedFilePath;

    void Start()
    {
        // uploadButton.onClick.AddListener(OpenFileBrowser);
        saveButton.onClick.AddListener(SaveImageToAWS);
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
            uploadedFilePath = paths[0];
            StartCoroutine(LoadTextureFromFile(uploadedFilePath));
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
                uploadedTexture = DownloadHandlerTexture.GetContent(uwr);
                if (uploadedTexture != null)
                {
                    Debug.Log("Texture loaded successfully.");
                }
                else
                {
                    Debug.LogError("Failed to create texture from file!");
                }
            }
        }
    }

    private void SaveImageToAWS()
    {
        if (uploadedTexture != null)
        {
            if (string.IsNullOrEmpty(folderName))
            {
                Debug.LogError("Folder name is not set.");
                return;
            }

            // Upload logic to AWS
            string organizationName = "YourOrganizationName"; // Replace with your organization name
            string path = $"{folderName}";

            // Assuming you have a method in SaveToAWS for uploading
            saveToAWS.UploadSingleImageToAWS(uploadedTexture, path);

            Debug.Log("Image uploaded to AWS.");
        }
        else
        {
            Debug.LogError("No texture to upload.");
        }
    }
}
