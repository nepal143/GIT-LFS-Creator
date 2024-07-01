using UnityEngine;
using TMPro;

public class DisplayName : MonoBehaviour
{
    public TMP_Text usernameText; // Assign this in the Inspector

    private void Start()
    {
        string username = PlayerPrefs.GetString("profileUsername"); // Get the username from PlayerPrefs
        usernameText.text = username; // Display the username on the TMP_Text component
    }
}