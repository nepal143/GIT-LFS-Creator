using UnityEngine;
using TMPro;
using System;

public class OrganisationVerificationUI : MonoBehaviour
{
    public APIManager apiManager;

    [SerializeField] private TMP_InputField verificationCodeField; 
    [SerializeField] private TMP_Text errorText;

    public void VerifyOrganisation()
    {
        string verificationCode = verificationCodeField.text;

        Action<string> callback = OnVerificationComplete;
        apiManager.VerifyOrganisation(verificationCode, callback);
    }

    private void OnVerificationComplete(string response)
    {
        if (response.Contains("Error"))
        {
            errorText.text = "Verification failed: " + response;
        }
        else
        {
            Debug.Log("Verification response: " + response);
            errorText.text = "Organisation verified successfully!";
        }
    }
}
