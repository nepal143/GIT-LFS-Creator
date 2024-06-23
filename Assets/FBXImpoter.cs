using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using SFB; // Standalone File Browser namespace
using Dummiesman; // Ensure this namespace is available
using TMPro; // Include TextMeshPro namespace

public class ModelImporterWithFileBrowser : MonoBehaviour
{
    public Button importButton;
    public Transform spawnPoint;
    public string dollHouseTag = "DollHouse"; // Tag to assign to the dollhouse
    public GameObject mainCanvas;
    public GameObject DollHouseRotationCanvas;
    public Camera mainCamera;
    public Camera DollHouseCamera;

    public GameObject loadingPanel;
    public TMP_Text loadingTextTMP; // Reference to TextMeshPro text

    private string baseUrl = "http://localhost:3000/user"; // Adjust base URL as needed

    void Start()
    {
        if (importButton != null)
        {
            importButton.onClick.AddListener(OpenFileBrowser);
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

    public void OpenFileBrowser()
    {
        var extensions = new[] {
            new ExtensionFilter("Model Files", "obj"),
        };
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open Model File", "", extensions, false);

        if (paths != null && paths.Length > 0)
        {
            string path = paths[0];
            Debug.Log("File selected: " + path);

            StartCoroutine(LoadAndMoveOBJ(path));
        }
        else
        {
            Debug.Log("No file selected.");
        }
    }

    IEnumerator LoadAndMoveOBJ(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Invalid file path.");
            yield break;
        }

        ShowLoading("Loading OBJ...");

        Debug.Log("Loading OBJ from path: " + path);

        if (!IsValidPath(path))
        {
            Debug.LogError("File path contains invalid characters.");
            HideLoading();
            yield break;
        }

        GameObject objModel = new OBJLoader().Load(path);
        if (objModel != null)
        {
            // Instantiate the OBJ model
            GameObject instantiatedModel = Instantiate(objModel, spawnPoint.position, spawnPoint.rotation);
            instantiatedModel.tag = dollHouseTag;
            Debug.Log($"OBJ model loaded, moved to spawn point, and tagged with {dollHouseTag}.");

            // Upload OBJ model to MongoDB
            yield return UploadToMongoDB(path);

            // Change screen to DollHouse mode
            ChangeScreenForDollHouse();
        }
        else
        {
            Debug.LogError("Failed to load OBJ model from path: " + path);
        }

        HideLoading();
    }

    IEnumerator UploadToMongoDB(string filePath)
    {
        byte[] objModelData = File.ReadAllBytes(filePath); // Read OBJ model bytes

        using (UnityWebRequest request = new UnityWebRequest($"{baseUrl}/upload-obj", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(objModelData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/octet-stream");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log("OBJ model uploaded to MongoDB.");
                string responseText = request.downloadHandler.text;
                Debug.Log("Server response: " + responseText);
            }
        }
    }

    bool IsValidPath(string path)
    {
        char[] invalidChars = Path.GetInvalidPathChars();
        foreach (char c in invalidChars)
        {
            if (path.Contains(c.ToString()))
            {
                return false;
            }
        }
        return true;
    }

    public void ChangeScreenForDollHouse()
    {
        mainCanvas.SetActive(false);
        DollHouseRotationCanvas.SetActive(true);
        mainCamera.gameObject.SetActive(false);
        DollHouseCamera.gameObject.SetActive(true);
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
       