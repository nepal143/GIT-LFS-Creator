using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VerifyButton : MonoBehaviour
{
    [SerializeField] private TMP_InputField phoneNumberField; 
    [SerializeField] private TMP_InputField verificationCodeField;
    [SerializeField] private TextMeshProUGUI responseText;
    [SerializeField] private APIManager apiManager;
    [SerializeField] private GameObject registrationParent; // Parent GameObject of registration UI
    [SerializeField] private GameObject loginParent; // Parent GameObject of login UI
    [SerializeField] private GameObject verification ; 


    private void Start()   
    {
        Button verifyButton = GetComponent<Button>();
        verifyButton.onClick.AddListener(VerifyPhoneNumber);
    }

    private void VerifyPhoneNumber()
    {
        string phoneNumber = phoneNumberField.text;
        string verificationCode = verificationCodeField.text;

        apiManager.VerifyPhoneNumber(phoneNumber, verificationCode, OnVerificationComplete);
    }

    private void OnVerificationComplete(string response)
    {
        responseText.text = response;
         if (response.Contains("success")) // Adjust this based on your success response message
        {
            registrationParent.SetActive(false); // Disable registration UI parent
            loginParent.SetActive(true); // Enable login UI parent
            verification.SetActive(false) ;
        }
    }
}
