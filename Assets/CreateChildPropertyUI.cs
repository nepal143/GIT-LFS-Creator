using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // Add this namespace to use SceneManager

public class CreateChildPropertyUI : MonoBehaviour
{
    public TMP_InputField propertyNameInput;

    public TMP_InputField childPropertyNameInput;
    public Button createPropertyButton;
    public TextMeshProUGUI resultText;
    public APIManager apiManager;

    private void Start()
    {
        createPropertyButton.onClick.AddListener(OnCreatePropertyButtonClicked);
    }

    private void OnCreatePropertyButtonClicked()
    {
        string organisationName = PlayerPrefs.GetString("organisationName");  // Get the organisation name from PlayerPrefs
        string propertyName = propertyNameInput.text;
        string childPropertyName = childPropertyNameInput.text;

        apiManager.CreateChildProperty(organisationName, propertyName, childPropertyName, OnCreatePropertyResponse);
    }

    private void OnCreatePropertyResponse(string response)
    {
        if (response == "Property created and added to the organisation successfully")
        {
            PlayerPrefs.SetString("parentPropertyName", propertyNameInput.text);
            SceneManager.LoadScene("parentDashboard");
        }
        else
        {
            resultText.text = response;
        }
    }
}
