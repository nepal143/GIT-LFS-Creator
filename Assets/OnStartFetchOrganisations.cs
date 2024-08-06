using System;
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
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private GameObject restrictedAccessObject; // Object to disable if not root user
    [SerializeField] private GameObject centeredObject; // The UI element to center horizontally

    void Start()
    {
        organisationName = PlayerPrefs.GetString("organisationName");
        currentUsername = PlayerPrefs.GetString("profileUsername"); // Get the current username from PlayerPrefs
        Debug.Log($"Starting fetch for organisation: {organisationName}, current user: {currentUsername}");

        StartCoroutine(GetOrganisationDetails(organisationName));
    }

    public void OnClickFetchOrganisationData()
    {
        Debug.Log("Fetch Organisation Data button clicked");
        StartCoroutine(GetOrganisationDetails(organisationName));
    }

    IEnumerator GetOrganisationDetails(string organisationName)
    {
        string url = $"{baseUrl}/organisation/organisation/{organisationName}";
        Debug.Log($"Requesting organisation details from URL: {url}");

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.DataProcessingError)
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
                    Debug.Log("Current Username: " + currentUsername);
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
        if (usernamesContainer == null || propertiesContainer == null || textPrefab == null || buttonPrefab == null)
        {
            Debug.LogError("Containers, text prefab, or button prefab not assigned.");
            return;
        }

        ClearContainer(usernamesContainer);
        ClearContainer(propertiesContainer);

        if (usernames != null)
        {
            Debug.Log("Displaying usernames:");
            foreach (string username in usernames)
            {
                Debug.Log($"Username: {username}");
                TextMeshProUGUI newTextMeshPro = Instantiate(textPrefab, usernamesContainer.transform);
                newTextMeshPro.text = username;

                RectTransform rt = newTextMeshPro.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(0, rt.sizeDelta.y * usernames.IndexOf(username));
            }
        }
        else
        {
            Debug.LogWarning("Usernames list is null.");
        }

        if (properties != null)
        {
            Debug.Log("Displaying properties:");
            foreach (string property in properties)
            {
                Debug.Log($"Property: {property}");
                Button newButton = Instantiate(buttonPrefab, propertiesContainer.transform);
                newButton.GetComponentInChildren<TextMeshProUGUI>().text = property;

                newButton.onClick.AddListener(() => OnPropertyButtonClick(property));

                RectTransform rt = newButton.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(0, rt.sizeDelta.y * properties.IndexOf(property));
            }
        }
        else
        {
            Debug.LogWarning("Properties list is null.");
        }
    }

    void ClearContainer(GameObject container)
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
}

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
