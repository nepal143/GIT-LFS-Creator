using UnityEngine;
using UnityEditor;

public class SceneExporter : MonoBehaviour
{
    public void ExportSceneToPackage()
    {
        // Define the path for the exported package
        string packagePath = "Assets/MyExports/ExportedScene.unitypackage";
        
        // Get the path of the current active scene
        string scenePath = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;

        // Check if the scene is saved, as unsaved scenes cannot be exported
        if (string.IsNullOrEmpty(scenePath))
        {
            Debug.LogError("The scene needs to be saved before exporting.");
            return;
        }

        // Export the scene and its dependencies to a package
        AssetDatabase.ExportPackage(new string[] { scenePath }, packagePath, ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Recurse);

        Debug.Log("Scene exported to " + packagePath);
    }
}
