using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shiftBetween : MonoBehaviour
{
   
    public GameObject MainCanvas ;
    public GameObject TempCanvas ;
    public GameObject MainCamera ;

    public void ShiftToPanoromas() { 
        MainCanvas.SetActive(false) ;
        TempCanvas.SetActive(true) ; 
        MainCamera.SetActive(false) ; 
    }
    public void ShiftToMainMenu() { 
        MainCanvas.SetActive(true) ;
        TempCanvas.SetActive(false) ; 
        MainCamera.SetActive(true) ; 
    }
}
