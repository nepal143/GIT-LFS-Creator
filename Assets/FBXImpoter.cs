using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using SFB; // Standalone File Browser namespace
using TMPro; // Include TextMeshPro namespace
using Dummiesman; // Include Dummiesman namespace

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

    private string baseUrl = "http://localhost:3000"; // Adjust base URL as needed

    // Temporary variables for username and property name
    private string username;
    private string propertyName;

    void Start()
    {
        Debug.Log("Script started.");

        // Retrieve saved username and property name
        username = PlayerPrefs.GetString("username", "");
        propertyName = PlayerPrefs.GetString("propertyName", "");

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

            // Convert to absolute path for consistency
            path = Path.GetFullPath(path);

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
        Shader shader = Shader.Find("Standard");
        if (shader == null)
        {
            Debug.LogError("Shader is null.");
            HideLoading();
            yield break;
        }

        Debug.Log("Loading OBJ from path: " + path);

        // Load OBJ model
        GameObject objModel = null;
        try
        {
            objModel = new OBJLoader().Load(path);
            Debug.Log("OBJ model loaded successfully.");
            AssignDefaultShader(objModel); // Assign default shader
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error loading OBJ: " + ex.Message);
            HideLoading();
            yield break;
        }

        if (objModel != null)
        {
            // Instantiate the OBJ model
            GameObject instantiatedModel = Instantiate(objModel, spawnPoint.position, spawnPoint.rotation);
            instantiatedModel.tag = dollHouseTag;
            Debug.Log($"OBJ model loaded, moved to spawn point, and tagged with {dollHouseTag}.");

            // Trigger upload to server
            StartCoroutine(TriggerUploadToServer(path, username, propertyName));

            // Change screen to DollHouse mode
            ChangeScreenForDollHouse();
        }
        else
        {
            Debug.LogError("Failed to load OBJ model from path: " + path);
        }

        HideLoading();
    }

    void AssignDefaultShader(GameObject objModel)
    {
        Shader defaultShader = Shader.Find("Standard");
        if (defaultShader == null)
        {
            Debug.LogError("Default shader not found. Make sure 'Standard' shader is included in the build.");
            return;
        }

        Renderer[] renderers = objModel.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            foreach (var material in renderer.materials)
            {
                if (material.shader == null)
                {
                    material.shader = defaultShader;
                    Debug.Log($"Assigned default shader to material: {material.name}");
                }
            }
        }
    }

    IEnumerator TriggerUploadToServer(string filePath, string username, string propertyName)
    {
        ShowLoading("Uploading to server...");

        // Prepare form data
        WWWForm form = new WWWForm();
        form.AddField("directoryPath", Path.GetDirectoryName(filePath)); // Send directory path
        form.AddField("username", username); // Send username
        form.AddField("propertyName", propertyName); // Send property name

        // Send request to server
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
        Debug.Log("Show loading: " + message);

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
        Debug.Log("Hide loading");

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }

        if (loadingTextTMP != null)
        {
            loadingTextTMP.gameObject.SetActive(false);
        }
    }

    public void ChangeScreenForDollHouse()
    {
        Debug.Log("Change screen to DollHouse mode");

        mainCanvas.SetActive(false);
        DollHouseRotationCanvas.SetActive(true);
        mainCamera.gameObject.SetActive(false);
        DollHouseCamera.gameObject.SetActive(true);
    }
}
