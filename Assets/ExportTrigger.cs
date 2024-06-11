using UnityEngine;
using UnityEngine.UI;

public class ExportButtonController : MonoBehaviour
{
    public Button exportButton;
    public SceneStateExport.SceneExporter exportObjectsScript;
    public SceneCleaner sceneCleanerObject;

    void Start()
    {
        if (exportButton != null)
        {
            exportButton.onClick.AddListener(OnExportButtonClicked);
        }
        else
        {
            Debug.LogError("Export button not assigned.");
        }
    }

    void OnExportButtonClicked()
    {
        if (exportObjectsScript != null)
        {
            sceneCleanerObject.CleanScene() ;
            exportObjectsScript.ExportRootObject();
        }
        else
        {
            Debug.LogError("ExportObjects script not assigned.");
        }
    }
}
      