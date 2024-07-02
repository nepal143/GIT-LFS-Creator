`using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;
using TMPro;

public class APIManager : MonoBehaviour
{
    private string baseUrl = "http://localhost:3000/";
    private string userId; // Store the user ID

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
                Debug.Log("Form upload complete!");
                string responseText = request.downloadHandler.text;
                var response = JsonUtility.FromJson<UserRegisterResponse>(responseText);
                userId = response.userId; // Store the user ID
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
                Debug.Log("Form upload complete!");
                string responseText = request.downloadHandler.text;
                callback(responseText);
            }
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
}
