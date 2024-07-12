using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    private string baseUrl = "http://localhost:3000/";
    private string userId;
    private string storedPhoneNumber;

    // Method to create a property
    public void CreateProperty(string organisationName, string parentPropertyName, string location, string description, string builderName, Action<string> callback)
    {
        StartCoroutine(CreatePropertyCoroutine(organisationName, parentPropertyName, location, description, builderName, callback));
    }

    private IEnumerator CreatePropertyCoroutine(string organisationName, string parentPropertyName, string location, string description, string builderName, Action<string> callback)
    {
        PropertyCreateData propertyData = new PropertyCreateData(organisationName, parentPropertyName, location, description, builderName);
        string jsonData = JsonUtility.ToJson(propertyData);

        using (UnityWebRequest request = new UnityWebRequest($"{baseUrl}create-property", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                callback(request.error);
            }
            else
            {
                Debug.Log("Property creation request completed!");
                string responseText = request.downloadHandler.text;
                callback(responseText);
            }
        }
    }

 public void RegisterOrganisation(string organisationName, string rootUsername, string password, string phoneNumber, Action<string> callback)
{
    storedPhoneNumber = phoneNumber;
    PlayerPrefs.SetString("rootPhoneNumber", phoneNumber); // Store the phone number in PlayerPrefs
    PlayerPrefs.SetString("organisationName", organisationName); // Store the organisation name in PlayerPrefs
    StartCoroutine(RegisterOrganisationCoroutine(organisationName, rootUsername, password, phoneNumber, callback));
}
    private IEnumerator RegisterOrganisationCoroutine(string organisationName, string rootUsername, string password, string phoneNumber, Action<string> callback)
    {
        OrganisationRegisterData registerData = new OrganisationRegisterData(organisationName, rootUsername, password, phoneNumber);
        string jsonData = JsonUtility.ToJson(registerData);

        using (UnityWebRequest request = new UnityWebRequest($"{baseUrl}organisation/create-organisation", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                callback(request.error);
            }
            else
            {
                Debug.Log("Organisation registration request completed!");
                string responseText = request.downloadHandler.text;
                callback(responseText);
            }
        }
    }

    public void VerifyOrganisation(string verificationCode, Action<string> callback)
    {
        StartCoroutine(VerifyOrganisationCoroutine(PlayerPrefs.GetString("rootPhoneNumber"), verificationCode, callback));
    }

    private IEnumerator VerifyOrganisationCoroutine(string phoneNumber, string verificationCode, Action<string> callback)
    {
        OrganisationVerificationData verificationData = new OrganisationVerificationData { phoneNumber = phoneNumber, verificationCode = verificationCode };
        string jsonData = JsonUtility.ToJson(verificationData);

        using (UnityWebRequest request = new UnityWebRequest($"{baseUrl}organisation/verify-root", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                callback(request.error);
            }
            else
            {
                Debug.Log("Organisation verification request completed!");
                string responseText = request.downloadHandler.text;
                callback(responseText);
            }
        }
    }

    public void RegisterUser(string username, string phoneNumber, string password, Action<string> callback)
    {
        StartCoroutine(RegisterUserCoroutine(username, phoneNumber, password, callback));
    }

    private IEnumerator RegisterUserCoroutine(string username, string phoneNumber, string password, Action<string> callback)
    {
        UserRegisterData registerData = new UserRegisterData { username = username, phoneNumber = phoneNumber, password = password };
        string jsonData = JsonUtility.ToJson(registerData);

        using (UnityWebRequest request = new UnityWebRequest($"{baseUrl}user/register", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                callback(request.error);
            }
            else
            {
                Debug.Log("User registration request completed!");
                string responseText = request.downloadHandler.text;
                var response = JsonUtility.FromJson<UserRegisterResponse>(responseText);
                userId = response.userId;
                callback(responseText);
            }
        }
    }

    public void VerifyPhoneNumber(string phoneNumber, string verificationCode, Action<string> callback)
    {
        StartCoroutine(VerifyPhoneNumberCoroutine(phoneNumber, verificationCode, callback));
    }

    private IEnumerator VerifyPhoneNumberCoroutine(string phoneNumber, string verificationCode, Action<string> callback)
    {
        VerificationData verificationData = new VerificationData { phoneNumber = phoneNumber, verificationCode = verificationCode };
        string jsonData = JsonUtility.ToJson(verificationData);

        using (UnityWebRequest request = new UnityWebRequest($"{baseUrl}user/verify", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                callback(request.error);
            }
            else
            {
                Debug.Log("Phone number verification request completed!");
                string responseText = request.downloadHandler.text;
                callback(responseText);
            }
        }
    }

    // Classes for serializing data to JSON
    [Serializable]
    public class PropertyCreateData
    {
        public string organisationName;
        public string parentPropertyName;
        public string location;
        public string description;
        public string builderName;

        public PropertyCreateData(string organisationName, string parentPropertyName, string location, string description, string builderName)
        {
            this.organisationName = organisationName;
            this.parentPropertyName = parentPropertyName;
            this.location = location;
            this.description = description;
            this.builderName = builderName;
        }
    }

    [Serializable]
    public class UserRegisterData
    {
        public string username;
        public string phoneNumber;
        public string password;
    }

    [Serializable]
    public class UserRegisterResponse
    {
        public string userId;
        public string verificationCode;
    }

    [Serializable]
    public class VerificationData
    {
        public string phoneNumber;
        public string verificationCode;
    }

    [Serializable]
    public class OrganisationRegisterData
    {
        public string organisationName;
        public string rootUsername;
        public string password;
        public string phoneNumber;

        public OrganisationRegisterData(string organisationName, string rootUsername, string password, string phoneNumber)
        {
            this.organisationName = organisationName;
            this.rootUsername = rootUsername;
            this.password = password;
            this.phoneNumber = phoneNumber;
        }
    }

    [Serializable]
    public class OrganisationVerificationData
    {
        public string phoneNumber;
        public string verificationCode;
    }
}
