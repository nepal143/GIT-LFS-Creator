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
            GameObject instantiatedModel = Instantiate(objModel, spawnPoint.position, spawnPoint.rotation);
            instantiatedModel.tag = dollHouseTag;
            Debug.Log($"OBJ model loaded, moved to spawn point, and tagged with {dollHouseTag}.");
            ChangeScreenForDollHouse();
        }
        else
        {
            Debug.LogError("Failed to load OBJ model from path: " + path);
        }

        HideLoading();
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
