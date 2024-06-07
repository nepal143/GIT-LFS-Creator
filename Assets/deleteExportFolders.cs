using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DeleteFolders : MonoBehaviour
{
    // Assign this in the Inspector
    public Button deleteButton;

    void Start()
    {
        // Add listener to the button to call DeleteFolders method on click
        // deleteButton.onClick.AddListener(DeleteSpecifiedFolders);
    }

    public void DeleteSpecifiedFolders()
    {
        // Construct the full paths to the folders inside the Assets folder
        string folderPath1 = Path.Combine(Application.dataPath, "ImportedImages");
        string folderPath2 = Path.Combine(Application.dataPath, "MyExports");

        DeleteFolder(folderPath1);
        DeleteFolder(folderPath2);
    }

    void DeleteFolder(string folderPath)
    {
        if (Directory.Exists(folderPath))
        {
            Directory.Delete(folderPath, true);
            Debug.Log("Deleted folder: " + folderPath);
        }
        else
        {
            Debug.LogWarning("Folder not found: " + folderPath);
        }
    }
}
