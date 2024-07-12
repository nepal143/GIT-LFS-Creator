using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateOrganisationCancle : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject CreateOrganisationUI;
    public GameObject DashBoardUI;

    public void OnClickCancle()
    {
        CreateOrganisationUI.SetActive(false);
        DashBoardUI.SetActive(true);
    }
}
