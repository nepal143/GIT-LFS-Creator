using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfPanaromaIsComplete : MonoBehaviour
{
    // Start is called before the firt frame update
    public GameObject AddPanaromaButton ;
    void Start()
    {
        gameObject.SetActive(false) ;
    }

    // Update is called once per frame
    void Update()
    {
        if(!AddPanaromaButton){
            gameObject.SetActive(true) ;
        }
    }
}
