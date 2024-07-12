using UnityEngine;

public class PlayerPrefsDebugger : MonoBehaviour
{
    void Start()
    {
        DebugAllPlayerPrefs();
    }

    void DebugAllPlayerPrefs()
    {
        // Get all keys
        string[] allKeys = PlayerPrefs.GetString("UnityPlayerPrefsKeys").Split(',');

        // Log all keys and their values
        foreach (string key in allKeys)
        {
            if (!string.IsNullOrEmpty(key))
            {
                string value = PlayerPrefs.GetString(key);
                Debug.Log($"Key: {key}, Value: {value}");
            }
        }
    }
}
