using UnityEngine;
using UnityEditor;

public class SceneExporter : MonoBehaviour
{
    public void ExportSceneToPackage()
    {
        // Define the path for the exported package
        string packagePath = "Assets/ExportedScene.unitypackage";

        // Find all root objects in the scene
        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        // Collect the paths of all root objects
        string[] assetPaths = new string[rootObjects.Length];
        for (int i = 0; i < rootObjects.Length; i++)
        {
            assetPaths[i] = AssetDatabase.GetAssetPath(rootObjects[i]);
        }

        // Export the scene objects to a package
        AssetDatabase.ExportPackage(assetPaths, packagePath, ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Recurse);

        Debug.Log("Scene exported to " + packagePath);
    }
}
