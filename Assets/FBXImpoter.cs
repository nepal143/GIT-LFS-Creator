using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using SFB; // Standalone File Browser namespace
using TMPro; // Include TextMeshPro namespace
using Dummiesman; // Include Dummiesman namespace
using Newtonsoft.Json;
using System.Collections.Generic;

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

    private string baseUrl = "https://theserver-tp6r.onrender.com/"; // Adjust base URL as needed

    private Shader standardShader;

    // Temporary variables for username and property name
    private string username;
    private string propertyName;
    private string childPropertyName;

    void Start()
    {
        childPropertyName = PlayerPrefs.GetString("childPropertyName");
        Debug.Log("Child Property Name: " + childPropertyName);

        username = PlayerPrefs.GetString("username", "");
        propertyName = PlayerPrefs.GetString("parentPropertyName", "");
        Debug.Log("Username: " + username);
        Debug.Log("Property Name: " + propertyName);

        if (importButton != null)
        {
            importButton.onClick.AddListener(OpenFileBrowser);
        }

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
        if (loadingTextTMP != null)
        {
            loadingTextTMP.gameObject.SetActive(false);
        }

        standardShader = Shader.Find("Standard");
        if (standardShader == null)
        {
            Debug.LogError("Standard shader not found. Ensure it is included in the build.");
        }
        else
        {
            Debug.Log("Standard shader loaded successfully.");
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

        GameObject objModel = null;
        try
        {
            objModel = new OBJLoader().Load(path);
            Debug.Log("OBJ model loaded successfully.");
            ApplyShaderToModel(objModel, standardShader); // Assign default shader

            // Move the model to the spawn point and tag it
            objModel.transform.position = spawnPoint.position;
            objModel.transform.rotation = spawnPoint.rotation;
            objModel.tag = dollHouseTag;

            Debug.Log($"OBJ model moved to spawn point and tagged with {dollHouseTag}.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error loading OBJ: " + ex.Message);
            HideLoading();
            yield break;
        }

        // Start the upload coroutine and wait for it to finish
        yield return StartCoroutine(TriggerUploadToServer(path, childPropertyName));

        HideLoading();
        // Change screen to DollHouse mode after all tasks are complete
        ChangeScreenForDollHouse();
    }

    void ApplyShaderToModel(GameObject model, Shader shader)
    {
        if (shader == null)
        {
            Debug.LogError("Shader is null. Cannot apply shader to model.");
            return;
        }

        Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                material.shader = shader;
            }
        }
        Debug.Log("Shader applied to model successfully.");
    }

    bool IsValidPath(string path)
    {
        char[] invalidChars = Path.GetInvalidPathChars();
        return path.IndexOfAny(invalidChars) == -1;
    }

    public IEnumerator TriggerUploadToServer(string filePath, string childPropertyName)
    {
        Debug.Log("Starting upload");
        ShowLoading("Uploading to server...");

        // Prepare form data
        WWWForm form = new WWWForm();
        form.AddField("organisationName", PlayerPrefs.GetString("organisationName"));
        form.AddField("parentpropertyName", PlayerPrefs.GetString("parentPropertyName"));
        form.AddField("childPropertyName", childPropertyName);

        // Add the file
        byte[] fileData = File.ReadAllBytes(filePath);
        Debug.Log($"Read file: {filePath}, file size: {fileData.Length} bytes");

        if (fileData == null || fileData.Length == 0)
        {
            Debug.LogError("File data is null or empty. Check the file path and ensure the file is readable.");
            HideLoading();
            yield break;
        }

        form.AddBinaryData("model", fileData, Path.GetFileName(filePath), "application/octet-stream");

        // Send request to server
        using (UnityWebRequest request = UnityWebRequest.Post($"{baseUrl}upload", form))
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
