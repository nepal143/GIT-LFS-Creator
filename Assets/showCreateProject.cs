using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showCreateProject : MonoBehaviour
{
    public GameObject TheCreateProject;
    public GameObject CreateScenebtn;

    public void showCreateProjectUI()
    {
        TheCreateProject.SetActive(true);
        CreateScenebtn.SetActive(false);
    }
    
}
