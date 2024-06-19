using UnityEngine;
using TMPro;
using System;

public class UserRegistrationUI : MonoBehaviour
{
    public APIManager apiManager;

    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField phoneNumberField;
    [SerializeField] private TMP_InputField passwordField;

    public void RegisterUser()
    {
        string username = usernameField.text;
        string phoneNumber = phoneNumberField.text;
        string password = passwordField.text;

        Action<string> callback = OnRegistrationComplete;

        apiManager.RegisterUser(username, phoneNumber, password, callback);
    }

    private void OnRegistrationComplete(string response)
    {
        Debug.Log("Registration response: " + response);
    }
}
