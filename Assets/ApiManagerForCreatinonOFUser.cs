using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityApiNamespaceForRegister
{
    public class ApiManagerForRegister : MonoBehaviour
    {
        private string baseUrl = "http://localhost:3000"; // Replace with your actual server URL

        // Register user
        public IEnumerator RegisterUser(string username, string phoneNumber, string password, string role, string organisationName)
        {
            string registerUrl = $"{baseUrl}/user/register";

            UserRegistrationData data = new UserRegistrationData
            {
                organisationName = organisationName,
                username = username,
                phoneNumber = phoneNumber,
                password = password, 
                role = role
            };

            string jsonData = JsonUtility.ToJson(data);
            byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonData);

            UnityWebRequest request = new UnityWebRequest(registerUrl, "POST");
            request.uploadHandler = new UploadHandlerRaw(postData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error registering user: {request.error}");
            }
            else
            {
                Debug.Log("User registered successfully");
            }
        }

        // Verify phone number
        public IEnumerator VerifyPhoneNumber(string phoneNumber, string verificationCode)
        {
            string verifyUrl = $"{baseUrl}/user/verify";

            VerificationData data = new VerificationData
            {
                phoneNumber = phoneNumber,
                verificationCode = verificationCode
            };

            string jsonData = JsonUtility.ToJson(data);
            byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonData);

            UnityWebRequest request = new UnityWebRequest(verifyUrl, "POST");
            request.uploadHandler = new UploadHandlerRaw(postData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error verifying phone number: {request.error}");
            }
            else
            {
                Debug.Log("Phone number verified successfully");
            }
        }
    }

    // User registration data class
    [System.Serializable]
    public class UserRegistrationData
    {
        public string organisationName;
        public string username;
        public string phoneNumber;
        public string password;
        public string role;
    }

    // Verification data class
    [System.Serializable]
    public class VerificationData
    {
        public string phoneNumber;
        public string verificationCode;
    }
}
