using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using SFB; // Standalone File Browser namespace
using Dummiesman; // Ensure this namespace is available

public class ModelImporterWithFileBrowser : MonoBehaviour
{
    public Button importButton;
    public Transform spawnPoint;
    public string dollHouseTag = "DollHouse"; // Tag to assign to the dollhouse
    public GameObject mainCanvas;
    public GameObject DollHouseRotationCanvas;
    public Camera mainCamera;
    public Camera DollHouseCamera;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);
#endif

    void Start()
    {
        if (importButton != null)
        {
            importButton.onClick.AddListener(OpenFileBrowser);
        }
    }

    public void OpenFileBrowser()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        UploadFile(gameObject.name, "OnFileSelected", ".fbx,.obj", false);
#else
        var extensions = new[] {
            new ExtensionFilter("Model Files", "fbx", "obj"),
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open Model File", "", extensions, false);

        if (paths.Length > 0)
        {
            string path = paths[0];
            if (path.EndsWith(".fbx"))
            {
                StartCoroutine(LoadAndMoveFBX(path));
            }
            else if (path.EndsWith(".obj"))
            {
                StartCoroutine(LoadAndMoveOBJ(path));
            }
        }
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    // Called from browser
    public void OnFileSelected(string url)
    {
        if (url.EndsWith(".fbx"))
        {
            StartCoroutine(LoadAndMoveFBX(url));
        }
        else if (url.EndsWith(".obj"))
        {
            StartCoroutine(LoadAndMoveOBJ(url));
        }
    }
#endif

    IEnumerator LoadAndMoveFBX(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Invalid file path.");
            yield break;
        }

        Debug.Log("Loading FBX from path: " + path);

        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle("file://" + path);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load AssetBundle from path: " + path + " Error: " + www.error);
            yield break;
        }

        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
        if (bundle == null)
        {
            Debug.LogError("Failed to load AssetBundle from path: " + path);
            yield break;
        }

        var assetNames = bundle.GetAllAssetNames();
        foreach (var assetName in assetNames)
        {
            GameObject fbxModel = bundle.LoadAsset<GameObject>(assetName);
            if (fbxModel != null)
            {
                // Wait until the model appears in the scene
                yield return new WaitForEndOfFrame();
                yield return new WaitForSeconds(1); // Add a delay to ensure the model appears

                // Find the model in the scene
                GameObject existingModel = GameObject.Find(fbxModel.name);
                if (existingModel != null)
                {
                    existingModel.transform.position = spawnPoint.position;
                    existingModel.transform.rotation = spawnPoint.rotation;
                    existingModel.tag = dollHouseTag; // Assign the tag
                    Debug.Log($"FBX model found, moved to spawn point, and tagged with {dollHouseTag}.");
                }
                else
                {
                    Debug.LogError("FBX model not found in the scene.");
                }
                break;
            }
        }
        bundle.Unload(false);

        // Change screen for doll house after loading the model
        changeScreenForDollHouse();
    }

    IEnumerator LoadAndMoveOBJ(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Invalid file path.");
            yield break;
        }

        Debug.Log("Loading OBJ from path: " + path);

        // Validate the path
        if (!IsValidPath(path))
        {
            Debug.LogError("File path contains invalid characters.");
            yield break;
        }

        // Load the OBJ model
        GameObject objModel = new OBJLoader().Load(path);
        if (objModel != null)
        {
            // Wait until the model appears in the scene
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(1); // Add a delay to ensure the model appears

            // Find the model in the scene
            GameObject existingModel = GameObject.Find(objModel.name);
            if (existingModel != null)
            {
                existingModel.transform.position = spawnPoint.position;
                existingModel.transform.rotation = spawnPoint.rotation;
                existingModel.tag = dollHouseTag; // Assign the tag
                Debug.Log($"OBJ model found, moved to spawn point, and tagged with {dollHouseTag}.");
            }
            else
            {
                Debug.LogError("OBJ model not found in the scene.");
            }
        }
        else
        {
            Debug.LogError("Failed to load OBJ model from path: " + path);
        }

        // Change screen for doll house after loading the model
        changeScreenForDollHouse();
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

    public void changeScreenForDollHouse()
    {
        mainCanvas.SetActive(false);
        DollHouseRotationCanvas.SetActive(true);
        mainCamera.gameObject.SetActive(false);
        DollHouseCamera.gameObject.SetActive(true);
    }
}
