using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeUI : MonoBehaviour
{
    public GameObject UI1 ; 
    public GameObject UI2 ;

    public void ChangeUI1()
    {
        UI1.SetActive(true);
        UI2.SetActive(false);
    }
    
}
