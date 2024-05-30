using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;

public class ExportButtonController : MonoBehaviour
{
    public Button exportButton;
    public SceneCleaner sceneCleaner;

    void Start()
    {
        if (exportButton != null)
        {
            exportButton.onClick.AddListener(OnExportButtonClicked);
        }
    }

    void OnExportButtonClicked()
    {
        if (sceneCleaner != null)
        {
            sceneCleaner.CleanScene();
            ExportSceneToPackage();
        }
        else
        {
            Debug.LogError("SceneCleaner component not found.");
        }
    }

    void ExportSceneToPackage()
    {
        // Define the path for the exported package
        string packagePath = "Assets/ExportedScene.unitypackage";

        // Export the scene to a Unity package
        AssetDatabase.ExportPackage("Assets", packagePath, ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Recurse);

        Debug.Log("Scene exported to " + packagePath);
    }
}
