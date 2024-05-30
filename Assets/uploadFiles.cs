using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uploadFiles : MonoBehaviour
{
   
    public GameObject button1 ; 
    public GameObject button2 ; 
    public GameObject Buttons ;
    public void clicked(){
        button1.SetActive(false) ;   
        
        button2.SetActive(false) ; 
        Buttons.SetActive(true) ; 
    }

    public void Back() {
        button1.SetActive(true) ;
        button2.SetActive(true) ; 
        Buttons.SetActive(false) ;
    }
}
