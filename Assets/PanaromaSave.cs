using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanaromaSave : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject MainCamera;
    public GameObject tempCanvas;
    public GameObject AddPanaromaButton;
    public GameObject MainCanvas;
    public GameObject button2;
    public GameObject Buttons;
    public GameObject ImageGridContainer ; 
    public GameObject PanaromaCompleteText ;

    public void OnClickSave()
    {
        MainCamera.SetActive(true);
        tempCanvas.SetActive(false);
        MainCanvas.SetActive(true);
        AddPanaromaButton.SetActive(false);
        button2.SetActive(true);
        Buttons.SetActive(false);
        ImageGridContainer.SetActive(false) ; 
        PanaromaCompleteText.SetActive(true) ; 
    }
}
