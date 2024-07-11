using UnityEngine;
using TMPro;
using System;

public class OrganisationRegistrationUI : MonoBehaviour
{
    public APIManager apiManager;

    [SerializeField] private TMP_InputField organisationNameField;
    [SerializeField] private TMP_InputField rootUsernameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_InputField confirmPasswordField;
    [SerializeField] private TMP_InputField phoneNumberField;
    [SerializeField] private TMP_Text errorText;

    public void RegisterOrganisation()
    {
        string organisationName = organisationNameField.text;
        string rootUsername = rootUsernameField.text;
        string password = passwordField.text;
        string confirmPassword = confirmPasswordField.text;
        string phoneNumber = phoneNumberField.text;

        // Check if passwords match
        if (password != confirmPassword)
        {
            errorText.text = "Passwords do not match!";
            return;
        }

        Action<string> callback = OnRegistrationComplete;
        apiManager.RegisterOrganisation(organisationName, rootUsername, password, phoneNumber, callback);
    }

    private void OnRegistrationComplete(string response)
    {
        Debug.Log("Registration response: " + response);
    }
}
