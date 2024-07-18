using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // Add this namespace to use SceneManager

public class CreatePropertyUI : MonoBehaviour
{
    public TMP_InputField propertyNameInput;
    public TMP_InputField locationInput;
    public TMP_InputField descriptionInput;
    public TMP_InputField builderNameInput;
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
        string location = locationInput.text;
        string description = descriptionInput.text;
        string builderName = builderNameInput.text;

        apiManager.CreateProperty(organisationName, propertyName, location, description, builderName, OnCreatePropertyResponse);
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
