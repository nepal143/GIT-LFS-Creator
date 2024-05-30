using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class saveAndBackInMovementDollHouse : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject movementButtons ; 
    public GameObject RotationButtons ; 
    public GameObject mainCanvas ;
    public GameObject DollHouseCanvas ; 
    public GameObject AddDollHouseButton ; 
    public GameObject FinalSaveScreen ; 

    public GameObject DollHouseCamera ; 
    public GameObject MainCamear ; 

    public void Save(){
        if(!movementButtons.activeSelf){
            movementButtons.SetActive(true);
            RotationButtons.SetActive(false) ;
        }
        else{
            movementButtons.SetActive(false);
            FinalSaveScreen.SetActive(true) ; 
            DollHouseCanvas.SetActive(false) ;
            MainCamear.SetActive(true) ; 
        }
    }

}
