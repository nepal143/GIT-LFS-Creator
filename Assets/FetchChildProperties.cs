using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FetchChildProperties : MonoBehaviour
{
    [SerializeField] private string baseUrl = "http://localhost:3000"; // Replace with your server URL
    [SerializeField] private TextMeshProUGUI parentPropertyNameTextObject; // Drag the new TextMeshPro object here in the Inspector
    [SerializeField] private GameObject childPropertiesContainer; // Drag the ChildPropertiesContainer here in the Inspector
    [SerializeField] private GameObject buttonContainerPrefab; // Drag the Button container prefab here in the Inspector
    [SerializeField] private Button propertyButtonPrefab; // Drag the property Button prefab here in the Inspector
    [SerializeField] private Button deleteButtonPrefab; // Drag the delete Button prefab here in the Inspector

    private string parentPropertyName;

    void Start()
    {
        parentPropertyName = PlayerPrefs.GetString("parentPropertyName");

        if (!string.IsNullOrEmpty(parentPropertyName))
        {
            StartCoroutine(GetChildProperties(parentPropertyName));
        }
        else
        {
            Debug.LogError("Parent property name is not set in PlayerPrefs.");
        }
    }

    public void OnClickFetchChildProperties()
    {
        parentPropertyName = PlayerPrefs.GetString("parentPropertyName");

        if (!string.IsNullOrEmpty(parentPropertyName))
        {
            StartCoroutine(GetChildProperties(parentPropertyName));
        }
        else
        {
            Debug.LogError("Parent property name is not set in PlayerPrefs.");
        }
    }

    IEnumerator GetChildProperties(string parentPropertyName)
    {
        string url = $"{baseUrl}/parentproperty/{parentPropertyName}/child-properties";
        Debug.Log($"Request URL: {url}");

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Send the request and wait for a response
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError($"Error fetching child properties: {webRequest.error}");
                Debug.LogError($"Response Code: {webRequest.responseCode}");
                Debug.LogError($"Response: {webRequest.downloadHandler.text}");
            }
            else
            {
                // Process the response
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log($"Child properties: {jsonResponse}");

                // Deserialize the JSON response into a list of strings
                List<string> childProperties = DeserializeJsonArray(jsonResponse);

                // Display the parent property name and child properties
                DisplayChildProperties(parentPropertyName, childProperties);
            }
        }
    }

    List<string> DeserializeJsonArray(string jsonArray)
    {
        string wrappedJson = $"{{\"childProperties\":{jsonArray}}}";
        ChildPropertiesResponse response = JsonUtility.FromJson<ChildPropertiesResponse>(wrappedJson);
        return response.childProperties;
    }

    void DisplayChildProperties(string parentPropertyName, List<string> childProperties)
    {
        // Display the parent property name using TextMeshPro
        if (parentPropertyNameTextObject != null)
        {
            parentPropertyNameTextObject.text = parentPropertyName;
        }
        else
        {
            Debug.LogWarning("Parent Property Name TextMeshPro object not assigned.");
        }

        // Display child properties
        DisplayProperties(childProperties);
    }

    void DisplayProperties(List<string> properties)
    {
        if (childPropertiesContainer == null || buttonContainerPrefab == null || propertyButtonPrefab == null || deleteButtonPrefab == null)
        {
            Debug.LogError("Child Properties container, button container prefab, property button prefab or delete button prefab not assigned.");
            return;
        }

        // Clear existing children
        ClearContainer(childPropertiesContainer);

        // Display properties
        if (properties != null)
        {
            foreach (string property in properties)
            {
                GameObject buttonContainer = Instantiate(buttonContainerPrefab, childPropertiesContainer.transform);

                Button propertyButton = buttonContainer.transform.Find("PropertyButton").GetComponent<Button>();
                TextMeshProUGUI buttonText = propertyButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = property;
                }
                else
                {
                    Debug.LogError("Property button prefab does not contain a TextMeshProUGUI component.");
                }

                Button deleteButton = buttonContainer.transform.Find("DeleteButton").GetComponent<Button>();
                TextMeshProUGUI deleteButtonText = deleteButton.GetComponentInChildren<TextMeshProUGUI>();
                if (deleteButtonText != null)
                {
                    deleteButtonText.text = "Delete";
                }
                else
                {
                    Debug.LogError("Delete button prefab does not contain a TextMeshProUGUI component.");
                }

                // Assign the property name to the buttons' click events
                propertyButton.onClick.AddListener(() => OnChildPropertyButtonClicked(property));
                deleteButton.onClick.AddListener(() => OnDeleteButtonClicked(property, buttonContainer));
            }
        }
        else
        {
            Debug.LogWarning("Properties list is null.");
        }
    }

    void ClearContainer(GameObject container)
    {
        foreach (Transform child in container.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void OnChildPropertyButtonClicked(string propertyName)
    {
        PlayerPrefs.SetString("childPropertyName", propertyName);
        SceneManager.LoadScene("CreateScene");
    }

    void OnDeleteButtonClicked(string propertyName, GameObject buttonContainer)
    {
        StartCoroutine(DeleteChildProperty(propertyName, buttonContainer));
    }

    IEnumerator DeleteChildProperty(string propertyName, GameObject buttonContainer)
    {
        string url = $"{baseUrl}/parentproperty/{parentPropertyName}/child-properties/{propertyName}";
        Debug.Log($"Delete Request URL: {url}");

        using (UnityWebRequest webRequest = UnityWebRequest.Delete(url))
        {
            // Send the request and wait for a response
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError($"Error deleting child property: {webRequest.error}");
                Debug.LogError($"Response Code: {webRequest.responseCode}");
                Debug.LogError($"Response: {webRequest.downloadHandler.text}");
            }
            else
            {
                Debug.Log($"Child property {propertyName} deleted successfully.");
                Destroy(buttonContainer);
            }
        }
    }

    // Class to deserialize the JSON response
    [System.Serializable]
    public class ChildPropertiesResponse
    {
        public List<string> childProperties;
    }
}
