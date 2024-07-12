using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateOrganisationUIFlow : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject organisationRegistrationUI;
    public GameObject OrganisationPage ; 

    // Update is called once per frame
    public void CreateOrganisationFlow()
    {
        organisationRegistrationUI.SetActive(true);
        OrganisationPage.SetActive(false);
    }
}
