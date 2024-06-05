using UnityEngine;
using UnityEngine.UI;

public class ExportButtonController : MonoBehaviour
{
    public Button exportButton;
    public SceneStateExporter exportObjectsScript;

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
            exportObjectsScript.ExportSceneState();
        }
        else
        {
            Debug.LogError("ExportObjects script not assigned.");
        }
    }
}
