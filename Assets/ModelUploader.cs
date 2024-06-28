using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using TMPro; // Include TextMeshPro namespace

public class ModelUploader : MonoBehaviour
{
    public Button uploadButton;
    public string dollHouseTag = "DollHouse"; // Tag to assign to the dollhouse
    public GameObject mainCanvas;
    public GameObject DollHouseRotationCanvas;
    public Camera mainCamera;
    public Camera DollHouseCamera;

    public GameObject loadingPanel;
    public TMP_Text loadingTextTMP; // Reference to TextMeshPro text

    private string baseUrl = "http://localhost:3000"; // Adjust base URL as needed

    // Temporary username and property name variables
    private string username = "tempUser";
    private string propertyName = "tempProperty";

    void Start()
    {
        if (uploadButton != null)
        {
            uploadButton.onClick.AddListener(UploadModelAndTexture);
        }

        // Hide loading panel and TMP text initially
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
        if (loadingTextTMP != null)
        {
            loadingTextTMP.gameObject.SetActive(false);
        }
    }

    public void UploadModelAndTexture()
    {
        StartCoroutine(UploadModelAndTextureCoroutine());
    }

    IEnumerator UploadModelAndTextureCoroutine()
    {
        ShowLoading("Uploading files...");

        GameObject objModel = GameObject.FindWithTag(dollHouseTag);
        if (objModel == null)
        {
            Debug.LogError("Model with tag 'DollHouse' not found in the scene.");
            HideLoading();
            yield break;
        }

        // Assuming the OBJ model and texture are stored in the Resources folder
        string modelPath = "Path/To/Your/OBJ/Model"; // Adjust the path as needed
        string texturePath = "Path/To/Your/Texture"; // Adjust the path as needed

        if (!File.Exists(modelPath) || !File.Exists(texturePath))
        {
            Debug.LogError("Model or texture file not found.");
            HideLoading();
            yield break;
        }

        byte[] modelData = File.ReadAllBytes(modelPath);
        byte[] textureData = File.ReadAllBytes(texturePath);
        string textureFileName = Path.GetFileName(texturePath);

        WWWForm form = new WWWForm();
        form.AddBinaryData("files", modelData, "model.obj", "application/octet-stream");
        form.AddBinaryData("files", textureData, textureFileName, "application/octet-stream");
        form.AddField("username", username);
        form.AddField("propertyName", propertyName);

        using (UnityWebRequest request = UnityWebRequest.Post($"{baseUrl}/upload", form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {request.error}, Response: {request.downloadHandler.text}");
            }
            else
            {
                Debug.Log("Files uploaded successfully.");
                Debug.Log("Response: " + request.downloadHandler.text);
            }
        }

        HideLoading();
    }

    void ShowLoading(string message)
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }

        if (loadingTextTMP != null)
        {
            loadingTextTMP.gameObject.SetActive(true);
            loadingTextTMP.text = message;
        }
    }

    void HideLoading()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }

        if (loadingTextTMP != null)
        {
            loadingTextTMP.gameObject.SetActive(false);
        }
    }
}
