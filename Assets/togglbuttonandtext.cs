using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import TextMesh Pro namespace

public class ToggleButtonText : MonoBehaviour
{
    public Button toggleButton;
    public TMP_Text toggleButtonText; // Change to TMP_Text
    public TMP_Text accountText; // Change to TMP_Text

    private bool isSignUp = true;

    void Start()
    {
        // Initialize button and text
        toggleButtonText.text = "Sign Up";
        accountText.text = "Have an account?";
        
        // Add listener for the button click event
        toggleButton.onClick.AddListener(ToggleText);
    }

    void ToggleText()
    {
        if (isSignUp)
        {
            toggleButtonText.text = "Login";
            accountText.text = "Don't have an account?";
        }
        else
        {
            toggleButtonText.text = "Sign Up";
            accountText.text = "Have an account?";
        }

        isSignUp = !isSignUp;
    }
}
