using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FetchOrganisationData : MonoBehaviour
{
    private string baseUrl = "https://theserver-tp6r.onrender.com"; // Replace with your server URL
    private string organisationName;
    private string currentUsername; // Current username of the logged-in user
    [SerializeField] private TextMeshProUGUI organisationNameTextObject;
    [SerializeField] private GameObject usernamesContainer;
    [SerializeField] private GameObject propertiesContainer;
    [SerializeField] private TextMeshProUGUI textPrefab;
    [SerializeField] private GameObject deleteButtonPrefab; // Serialized field for delete button
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private GameObject restrictedAccessObject; // Object to disable if not root user
    [SerializeField] private GameObject centeredObject; // The UI element to center horizontally

    [System.Serializable]
    public class Organisation
    {
        public string OrganisationName;
        public string RootUserName;
        public List<string> Usernames;
        public List<string> Properties;
    }

    void Start()
    {
        organisationName = PlayerPrefs.GetString("organisationName");
        currentUsername = PlayerPrefs.GetString("profileUsername"); // Get the current username from PlayerPrefs
        Debug.Log($"Starting fetch for organisation: {organisationName}, current user: {currentUsername}");

        if (!string.IsNullOrEmpty(organisationName))
        {
            StartCoroutine(GetOrganisationDetails(organisationName));
        }
        else
        {
            Debug.LogError("Organisation name is not set in PlayerPrefs.");
        }
    }

    public void OnClickFetchOrganisationData()
    {
        Debug.Log("Fetch Organisation Data button clicked");
        if (!string.IsNullOrEmpty(organisationName))
        {
            StartCoroutine(GetOrganisationDetails(organisationName));
        }
        else
        {
            Debug.LogError("Organisation name is not set in PlayerPrefs.");
        }
    }

    IEnumerator GetOrganisationDetails(string organisationName)
    {
        string url = $"{baseUrl}/organisation/organisation/{UnityWebRequest.EscapeURL(organisationName)}";
        Debug.Log($"Requesting organisation details from URL: {url}");

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error fetching organisation details: {webRequest.error}");
                Debug.LogError($"Response Code: {webRequest.responseCode}");
                Debug.LogError($"Response: {webRequest.downloadHandler.text}");
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log($"Received organisation details: {jsonResponse}");

                Organisation organisation = JsonUtility.FromJson<Organisation>(jsonResponse);

                // Debug information for organisation details
                Debug.Log($"Organisation Name: {organisation.OrganisationName}");
                Debug.Log($"Root Username: {organisation.RootUserName}");

                DisplayOrganisationDetails(organisation.OrganisationName, organisation.Usernames, organisation.Properties);

                // Disable the restricted access object if the current user is not the root user
                if (currentUsername != organisation.RootUserName)
                {
                    Debug.Log(currentUsername) ; 
                    Debug.Log("Current user is not the root user. Disabling restricted access object.");
                    if (restrictedAccessObject != null)
                    {
                        restrictedAccessObject.SetActive(false);
                    }
                    else
                    {
                        Debug.LogWarning("Restricted access object is not assigned.");
                    }

                    // Center the additional UI element horizontally
                    if (centeredObject != null)
                    {
                        RectTransform rectTransform = centeredObject.GetComponent<RectTransform>();
                        rectTransform.anchorMin = new Vector2(0.5f, rectTransform.anchorMin.y);
                        rectTransform.anchorMax = new Vector2(0.5f, rectTransform.anchorMax.y);
                        rectTransform.anchoredPosition = new Vector2(0, rectTransform.anchoredPosition.y);
                        Debug.Log("Centered the additional UI element horizontally.");
                    }
                    else
                    {
                        Debug.LogWarning("Centered object is not assigned.");
                    }
                }
                else
                {
                    Debug.Log("Current user is the root user. Restricted access object remains active.");
                }
            }
        }
    }

    void DisplayOrganisationDetails(string organisationName, List<string> usernames, List<string> properties)
    {
        Debug.Log("Displaying organisation details");
        if (organisationNameTextObject != null)
        {
            organisationNameTextObject.text = organisationName;
            Debug.Log($"Organisation Name displayed: {organisationName}");
        }
        else
        {
            Debug.LogWarning("Organisation Name TextMeshPro object not assigned.");
        }

        DisplayUsernamesAndProperties(usernames, properties);
    }

    void DisplayUsernamesAndProperties(List<string> usernames, List<string> properties)
{
    if (usernamesContainer == null || propertiesContainer == null || textPrefab == null || deleteButtonPrefab == null || buttonPrefab == null)
    {
        Debug.LogError("Containers, text prefab, delete button prefab, or button prefab not assigned.");
        return;
    }

    ClearContainer(usernamesContainer);
    ClearContainer(propertiesContainer);

    if (usernames != null && usernames.Count > 0)
    {
        Debug.Log("Displaying usernames:");
        foreach (string username in usernames)
        {
            Debug.Log($"Username: {username}");

            GameObject usernameRow = new GameObject("UsernameRow");
            usernameRow.transform.SetParent(usernamesContainer.transform, false);
            RectTransform usernameRowTransform = usernameRow.AddComponent<RectTransform>();
            usernameRowTransform.sizeDelta = new Vector2(0, 30); // Adjust height as needed
            usernameRowTransform.anchorMin = new Vector2(0, 1);  // Top-left corner
            usernameRowTransform.anchorMax = new Vector2(1, 1);  // Top-right corner
            usernameRowTransform.pivot = new Vector2(0.5f, 1);   // Top-middle pivot

            // Display the username
            TextMeshProUGUI newTextMeshPro = Instantiate(textPrefab, usernameRow.transform);
            newTextMeshPro.text = username;
            RectTransform rt = newTextMeshPro.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0.5f);
            rt.anchorMax = new Vector2(0, 0.5f);
            rt.pivot = new Vector2(0, 0.5f);
            rt.anchoredPosition = new Vector2(10, 0); // Adjust position as needed

            // Add delete button next to the username
            GameObject deleteButtonObject = Instantiate(deleteButtonPrefab, usernameRow.transform);
            Button deleteButton = deleteButtonObject.GetComponent<Button>();
            deleteButton.onClick.AddListener(() => OnDeleteButtonClick(username));
            RectTransform deleteButtonRt = deleteButton.GetComponent<RectTransform>();
            deleteButtonRt.anchorMin = new Vector2(1, 0.5f);
            deleteButtonRt.anchorMax = new Vector2(1, 0.5f);
            deleteButtonRt.pivot = new Vector2(1, 0.5f);
            deleteButtonRt.anchoredPosition = new Vector2(-10, 0); // Adjust position as needed
        }
    }
    else
    {
        Debug.LogWarning("Usernames list is null or empty.");
    }

    if (properties != null && properties.Count > 0)
    {
        Debug.Log("Displaying properties:");
        foreach (string property in properties)
        {
            Debug.Log($"Property: {property}");
            Button newButton = Instantiate(buttonPrefab, propertiesContainer.transform);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = property;

            newButton.onClick.AddListener(() => OnPropertyButtonClick(property));

            RectTransform rt = newButton.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, -rt.sizeDelta.y * properties.IndexOf(property));
        }
    }
    else
    {
        Debug.LogWarning("Properties list is null or empty.");
    }
}    void ClearContainer(GameObject container)
    {
        Debug.Log($"Clearing container: {container.name}");
        foreach (Transform child in container.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void OnPropertyButtonClick(string propertyName)
    {
        Debug.Log($"Property button clicked: {propertyName}");
        PlayerPrefs.SetString("parentPropertyName", propertyName);
        SceneManager.LoadScene("parentDashboard");
    }

    void OnDeleteButtonClick(string username)
    {
        Debug.Log($"Delete button clicked for username: {username}");
        StartCoroutine(DeleteUsername(organisationName, username));
    }

    public IEnumerator DeleteUsername(string organisationName, string username)
{
    string deleteUrl = "https://theserver-tp6r.onrender.com/organisation/delete-username";

    // Create the JSON data object
    var data = new
    {
        organisationName = organisationName,
        username = username
    };

    // Serialize the object to JSON
    string jsonData = JsonUtility.ToJson(data);
    byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonData);

    // Create the request
    UnityWebRequest request = new UnityWebRequest(deleteUrl, "POST");
    request.uploadHandler = new UploadHandlerRaw(postData);
    request.downloadHandler = new DownloadHandlerBuffer();
    request.SetRequestHeader("Content-Type", "application/json");

    // Send the request and wait for the response
    yield return request.SendWebRequest();

    // Check for errors
    if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    {
        Debug.LogError($"Error deleting username: {request.error}");
        Debug.LogError($"Response Code: {request.responseCode}");
        Debug.LogError($"Response: {request.downloadHandler.text}");
    }
    else
    {
        Debug.Log("Username deleted successfully");
        Debug.Log($"Response Code: {request.responseCode}");
        Debug.Log($"Response: {request.downloadHandler.text}");
    }
}

}
