using UnityEngine;
using UnityEngine.UI;
using TMPro;
// using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LoginTrigger : MonoBehaviour
{
    public TMP_InputField usernameInputField; // Reference to the username input field
    public TMP_InputField passwordInputField; // Reference to the password input field
    public Button loginButton; // Reference to the login button

    private LoginManager loginManager;

    void Start()
    {
        loginManager = GetComponent<LoginManager>();

        if (loginButton!= null)
        {
            loginButton.onClick.AddListener(OnLoginButtonClick);
        }
        else
        {
            Debug.LogError("Login button is not assigned.");
        }
    }

    void OnLoginButtonClick()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Username or password is empty.");
            return;
        }

        loginManager.LoginUser(username, password, OnLoginResponse);
    }

void OnLoginResponse(string response)
{
    Debug.Log("Login Response: " + response);

    // Check if the response contains "bad"
    if (response.Contains("bad"))
    {
        // Handle login failure, e.g. display an error message
        Debug.LogError("Login failed: " + response);
        return;
    }

    // Parse the JSON response
    Dictionary<string, string> jsonData = JsonUtility.FromJson<Dictionary<string, string>>(response);

    // Check if the token is not null or empty
    if (jsonData.ContainsKey("token") &&!string.IsNullOrEmpty(jsonData["token"]))
    {
        // Load the next scene in the build index
    }
    else
    {
        // Handle login failure, e.g. display an error message
        Debug.LogError("Login failed: " + response);
    }
}
}