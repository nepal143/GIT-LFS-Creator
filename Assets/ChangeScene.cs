using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneLoader : MonoBehaviour
{
    private void Start()
    {
        // Disable the button initially if there are no scenes to load
        GetComponent<UnityEngine.UI.Button>().interactable = SceneManager.sceneCountInBuildSettings > 1;
    }

    public void LoadNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = (currentIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextIndex);
    }
}
