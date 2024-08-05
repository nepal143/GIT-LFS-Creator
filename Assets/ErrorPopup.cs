using UnityEngine;
using UnityEngine.UI;

public class ErrorPopup : MonoBehaviour
{
    public GameObject errorPopupPanel;
    public Text errorMessageText;
    public Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(ClosePopup);
        errorPopupPanel.SetActive(false);
    }

    public void ShowError(string message)
    {
        errorMessageText.text = message;
        errorPopupPanel.SetActive(true);
    }

    private void ClosePopup()
    {
        errorPopupPanel.SetActive(false);
    }
}
