using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    private string baseUrl = "http://localhost:3000/";
    private string jwtToken;
    private string storedUsername;
    private string organisationName;

    public void LoginUser(string username, string password, Action<string> callback)
    {
        StartCoroutine(LoginUserCoroutine(username, password, callback));
    }

    private IEnumerator LoginUserCoroutine(string username, string password, Action<string> callback)
    {
        UserLoginData loginData = new UserLoginData { username = username, password = password };
        string jsonData = JsonUtility.ToJson(loginData);

        using (UnityWebRequest request = new UnityWebRequest($"{baseUrl}user/login", "POST"))
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
                Debug.Log("Login successful!");

                // Parse the response to get token and organisation name
                UserLoginResponse loginResponse = JsonUtility.FromJson<UserLoginResponse>(request.downloadHandler.text);
                jwtToken = loginResponse.token;
                organisationName = loginResponse.organisationName;

                storedUsername = username; // Store the username
                PlayerPrefs.SetString("username", storedUsername); // Save username for future use
                PlayerPrefs.SetString("organisationName", organisationName); // Save organisation name for future use

                // Save the username for the profile view
                PlayerPrefs.SetString("profileUsername", username);

                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                int nextSceneIndex = currentSceneIndex + 1;
                SceneManager.LoadScene(nextSceneIndex);
            }
        }
    }

    [Serializable]
    public class UserLoginData
    {
        public string username;
        public string password;
    }

    [Serializable]
    public class UserLoginResponse
    {
        public string token;
        public string organisationName; // Add organisationName field
    }
}
