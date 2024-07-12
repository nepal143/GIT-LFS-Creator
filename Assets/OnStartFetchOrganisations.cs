using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro; // Add TextMeshPro namespace

public class FetchOrganisationData : MonoBehaviour
{
    [SerializeField] private string baseUrl = "http://localhost:3000"; // Replace with your server URL
    [SerializeField] private string organisationName ; // Replace with the organisation name
    [SerializeField] private TextMeshProUGUI organisationNameTextObject; // Drag the new TextMeshPro object here in the Inspector
    [SerializeField] private GameObject usernamesContainer; // Drag the UsernamesContainer here in the Inspector
    [SerializeField] private GameObject propertiesContainer; // Drag the PropertiesContainer here in the Inspector
    [SerializeField] private TextMeshProUGUI textPrefab; // Drag the TextMeshPro prefab here in the Inspector

    void Start()
    {
        StartCoroutine(GetOrganisationDetails(organisationName));
        organisationName = PlayerPrefs.GetString("organisationName");
    }

    IEnumerator GetOrganisationDetails(string organisationName)
    {
        string url = $"{baseUrl}/organisation/organisation/{organisationName}";
        Debug.Log($"Request URL: {url}");

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Send the request and wait for a response
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError($"Error fetching organisation details: {webRequest.error}");
                Debug.LogError($"Response Code: {webRequest.responseCode}");
                Debug.LogError($"Response: {webRequest.downloadHandler.text}");
            }
            else
            {
                // Process the response
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log($"Organisation details: {jsonResponse}");

                // Deserialize the JSON response into a C# object
                Organisation organisation = JsonUtility.FromJson<Organisation>(jsonResponse);

                // Display the organisation name, usernames, and properties
                DisplayOrganisationDetails(organisation.OrganisationName, organisation.Usernames, organisation.Properties);
            }
        }
    }

    void DisplayOrganisationDetails(string organisationName, List<string> usernames, List<string> properties)
    {
        // Display the organisation name using TextMeshPro
        if (organisationNameTextObject != null)
        {
            organisationNameTextObject.text = organisationName;
        }
        else
        {
            Debug.LogWarning("Organisation Name TextMeshPro object not assigned.");
        }

        // Display usernames and properties
        DisplayUsernamesAndProperties(usernames, properties);
    }

    void DisplayUsernamesAndProperties(List<string> usernames, List<string> properties)
{
    if (usernamesContainer == null || propertiesContainer == null || textPrefab == null)
    {
        Debug.LogError("Containers or text prefab not assigned.");
        return;
    }

    // Clear existing children
    ClearContainer(usernamesContainer);
    ClearContainer(propertiesContainer);

    // Display usernames
    if (usernames != null)
    {
        foreach (string username in usernames)
        {
            TextMeshProUGUI newTextMeshPro = Instantiate(textPrefab, usernamesContainer.transform);
            newTextMeshPro.text = username;

            // Optional: Adjust position using RectTransform
            RectTransform rt = newTextMeshPro.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0,rt.sizeDelta.y * usernames.IndexOf(username)); // Adjust position based on index
        }
    }
    else
    {
        Debug.LogWarning("Usernames list is null.");
    }

    // Display properties
    if (properties != null)
    {
        foreach (string property in properties)
        {
            TextMeshProUGUI newTextMeshPro = Instantiate(textPrefab, propertiesContainer.transform);
            newTextMeshPro.text = property;

            // Optional: Adjust position using RectTransform
            RectTransform rt = newTextMeshPro.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, rt.sizeDelta.y * properties.IndexOf(property)); // Adjust position based on index
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
}

// Define a class to match the JSON response structure if you want to deserialize it
[System.Serializable]
public class Organisation
{
    public string OrganisationName;
    public string RootUserName;
    public string password;
    public string phoneNumber;
    public string verificationCode;
    public bool isRootVerified;
    public List<string> Usernames;
    public List<string> Properties;
}
