// using UnityEngine;
// using UnityEngine.SceneManagement;
// using System.IO;
// using SFB; // Import StandaloneFileBrowser namespace

// public class SceneExporter : MonoBehaviour
// {
//     public GameObject[] objectsToExclude;

//     public void ExportScene()
//     {
//         // Disable objects to be excluded
//         foreach (GameObject obj in objectsToExclude)
//         {
//             obj.SetActive(false);
//         }

//         // Prompt user to choose a save location
//         string[] extensions = new[] {
//             new ExtensionFilter("JSON Files", "json"),
//             // Add more filters if needed
//         };
//         string savePath = StandaloneFileBrowser.SaveFilePanel("Export Scene", "", SceneManager.GetActiveScene().name, extensions);

//         if (string.IsNullOrEmpty(savePath))
//         {
//             Debug.Log("Export canceled by user.");
//             return;
//         }

//         // Serialize scene data
//         string sceneData = JsonUtility.ToJson(SceneManager.GetActiveScene());

//         // Save scene data to the chosen file path
//         File.WriteAllText(savePath, sceneData);

//         Debug.Log("Scene exported successfully after excluding specified objects.");

//         // Re-enable excluded objects
//         foreach (GameObject obj in objectsToExclude)
//         {
//             obj.SetActive(true);
//         }
//     }
// }
