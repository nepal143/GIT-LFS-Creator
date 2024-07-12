using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VerifyButton : MonoBehaviour
{
    [SerializeField] private TMP_InputField phoneNumberField; 
    [SerializeField] private TMP_InputField verificationCodeField;
    [SerializeField] private TextMeshProUGUI responseText;
    [SerializeField] private APIManager apiManager;
    [SerializeField] private GameObject CreateUserUI;
    [SerializeField] private GameObject DashBoardUI;


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
            CreateUserUI.SetActive(false);
            DashBoardUI.SetActive(true);
        }
    }
}
