
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomInputField : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Image inputFieldBackground;
    [SerializeField] Sprite normalSprite;
    [SerializeField] Sprite selectedSprite;

    void Start()
    {
        inputField.onValueChanged.AddListener(OnTextChanged);
        inputField.onEndEdit.AddListener(OnEndEdit);
    }

    void OnTextChanged(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            inputFieldBackground.sprite = selectedSprite;
        }
    }

    void OnEndEdit(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            inputFieldBackground.sprite = selectedSprite;
        }
        else
        {
            inputFieldBackground.sprite = normalSprite;
        }
    }
}
