using UnityEngine;
using System.Collections;
using TMPro; // Import TextMeshPro namespace
using UnityApiNamespaceForRegister; // Import the new namespace for ApiManagerForRegister

namespace UnityUiNamespace
{
    public class UITrigger : MonoBehaviour
    {
        public TMP_InputField usernameInput;
        public TMP_InputField phoneNumberInput;
        public TMP_InputField passwordInput;
        public ApiManagerForRegister apiManagerForRegister; // Take ApiManagerForRegister as an input field

        private string organisationName; // To store organisation name

        void Start()
        {
            // Verify if components are assigned
            if (usernameInput == null || phoneNumberInput == null || passwordInput == null || apiManagerForRegister == null)
            {
                Debug.LogError("One or more input fields or ApiManagerForRegister are not assigned in the Inspector.");
                return;
            }

            organisationName = PlayerPrefs.GetString("organisationName");
            if (string.IsNullOrEmpty(organisationName))
            {
                Debug.LogWarning("OrganisationName not found in PlayerPrefs.");
            }
        }

        // Trigger to register a new user
        public void OnRegisterUser()
        {
            string username = usernameInput.text.Trim();
            string phoneNumber = phoneNumberInput.text.Trim();
            string password = passwordInput.text;
            string role = "Creator"; // Assuming role is predefined for registration

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(phoneNumber) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(role))
            {
                StartCoroutine(apiManagerForRegister.RegisterUser(username, phoneNumber, password, role, organisationName)); // Pass organisationName to RegisterUser
            }
            else
            {
                Debug.LogWarning("Please fill in all fields");
            }
        }

        // Trigger to verify user's phone number
        public void OnVerifyPhoneNumber()
        {
            string phoneNumber = phoneNumberInput.text.Trim();
            string verificationCode = "123456"; // Replace with actual verification code input from UI

            if (!string.IsNullOrEmpty(phoneNumber) && !string.IsNullOrEmpty(verificationCode))
            {
                StartCoroutine(apiManagerForRegister.VerifyPhoneNumber(phoneNumber, verificationCode));
            }
            else
            {
                Debug.LogWarning("Please enter phone number and verification code");
            }
        }
    }
}
