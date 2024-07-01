using UnityEngine;
using TMPro;

public class SaveUsername : MonoBehaviour
{
    public TMP_InputField propertynameInput; // Assign this in the Inspector

    public void SaveUsernameButtonClicked()
    {
        string propertyName = propertynameInput.text;
        PlayerPrefs.SetString("propertyName", propertynameInput.text);
        Debug.Log("Username saved: " + propertyName );
        PlayerPrefs.Save();
    }
}