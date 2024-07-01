using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoutButton : MonoBehaviour
{
    void Start()
    {
        string username = PlayerPrefs.GetString("profileUsername");

        if (string.IsNullOrEmpty(username))
        {
            SceneManager.LoadScene(0);
        }
    }

    public void LogoutButtonClicked()
    {
        string username = PlayerPrefs.GetString("profileUsername");
        string propertyName = PlayerPrefs.GetString("propertyName");

        Debug.Log("Before logout: username = " + username + ", propertyName = " + propertyName);

        PlayerPrefs.DeleteKey("profileUsername");
        PlayerPrefs.DeleteKey("propertyName");
        PlayerPrefs.Save();

        Debug.Log("After logout: username = " + PlayerPrefs.GetString("profileUsername") + ", propertyName = " + PlayerPrefs.GetString("propertyName"));

        // Load the scene at build index 0
        SceneManager.LoadScene(0);
    }
}