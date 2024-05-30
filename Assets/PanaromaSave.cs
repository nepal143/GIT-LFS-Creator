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
    public GameObject FinalSaveScene ; 
    public GameObject AddDollHoues ;

    public void OnClickSave()
    {
        
        GameObject dollHouse = GameObject.FindGameObjectWithTag("DollHouse");

        tempCanvas.SetActive(false);
        MainCamera.SetActive(true);
        AddPanaromaButton.SetActive(false);
        if(!dollHouse)
        {
        button2.SetActive(true);
        MainCanvas.SetActive(true);
        Buttons.SetActive(false);
        ImageGridContainer.SetActive(false) ; 
        PanaromaCompleteText.SetActive(true) ; 

        }
        else{
            FinalSaveScene.SetActive(true) ; 
        }
    }
}
