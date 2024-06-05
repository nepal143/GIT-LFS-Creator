using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class SceneStateExporter : MonoBehaviour
{
    public string exportFolderPath = "Assets/MyExports";
    public string exportFileName = "ExportedScene.unitypackage";

    public void ExportSceneState()
    {
        // Ensure the export directory exists
        if (!AssetDatabase.IsValidFolder(exportFolderPath))
        {
            AssetDatabase.CreateFolder("Assets", "MyExports");
        }

        // Collect the paths of the objects to export
        List<string> assetPaths = new List<string>();

        // Collect images
        SaveObjectsWithTag("Image", assetPaths);

        // Collect hotspots
        SaveObjectsWithTag("Hotspot", assetPaths);

        // Collect dollhouse model
        SaveObjectsWithTag("DollHouse", assetPaths);

        // Export the selected objects to a package
        string packagePath = Path.Combine(exportFolderPath, exportFileName);
        AssetDatabase.ExportPackage(assetPaths.ToArray(), packagePath, ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Recurse);

        Debug.Log("Selected objects exported to " + packagePath);
    }

    private void SaveObjectsWithTag(string tag, List<string> assetPaths)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in objects)
        {
            string assetPath = SaveObjectAsPrefab(obj);
            if (!string.IsNullOrEmpty(assetPath))
            {
                assetPaths.Add(assetPath);
            }
        }
    }

    private string SaveObjectAsPrefab(GameObject obj)
    {
        string path = Path.Combine(exportFolderPath, obj.name + ".prefab");
        Object prefab = PrefabUtility.SaveAsPrefabAsset(obj, path);
        if (prefab != null)
        {
            return path;
        }
        else
        {
            Debug.LogError("Failed to save prefab for " + obj.name);
            return null;
        }
    }
}
