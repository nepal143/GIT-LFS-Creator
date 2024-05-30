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
    public GameObject MainCamera ; 
    public GameObject AddPanaroma ; 

    public void Save(){
        if(!movementButtons.activeSelf){
            movementButtons.SetActive(true);
            RotationButtons.SetActive(false) ;
        }
        else{
            if(!AddPanaroma.activeSelf){
            movementButtons.SetActive(false);
            FinalSaveScreen.SetActive(true) ; 
            DollHouseCanvas.SetActive(false) ;
            MainCamera.SetActive(true) ; 

            }
            else{
                    mainCanvas.SetActive(true);
                    DollHouseCanvas.SetActive(false);
                    mainCanvas.SetActive(true) ;
                    MainCamera.gameObject.SetActive(true);
                    DollHouseCamera.gameObject.SetActive(true);
                    AddDollHouseButton.SetActive(false);
            }
        }
    }

}
