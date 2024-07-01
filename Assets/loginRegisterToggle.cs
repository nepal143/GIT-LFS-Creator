using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleLoginRegisterButton : MonoBehaviour
{
    public GameObject loginUI;
    public GameObject registerUI;
    public Button toggleButton;
    public TextMeshProUGUI toggleButtonText;

    private bool isLoginVisible = true;

    void Start()
    {
        toggleButton.onClick.AddListener(ToggleButtonClick);
        UpdateToggleButtonText();
    }

    void ToggleButtonClick()
    {
        isLoginVisible =!isLoginVisible;
        ToggleUI();
        UpdateToggleButtonText();
    }

    void ToggleUI()
    {
        loginUI.SetActive(isLoginVisible);
        registerUI.SetActive(!isLoginVisible);
    }

    void UpdateToggleButtonText()
    {
        toggleButtonText.text = isLoginVisible? "Sign Up" : "Log In";
    }
}