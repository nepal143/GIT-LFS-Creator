using UnityEngine;

public class SphereGenerator : MonoBehaviour
{
    public GameObject spherePrefab;
    public string cameraTagName = "tempCam";
    public PanoramaSelectionUI panoramaSelectionUI; // Reference to the PanoramaSelectionUI script

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
        {
            GenerateSphere();
        }
    }

    void GenerateSphere()
    {
        GameObject cameraObject = GameObject.FindGameObjectWithTag(cameraTagName);

        if (cameraObject != null)
        {
            Vector3 spawnPosition = cameraObject.transform.position + cameraObject.transform.forward * 10f;
            GameObject hotspot = Instantiate(spherePrefab, spawnPosition, Quaternion.identity);
            
            // Ensure the HotspotScript is attached to the new hotspot
            if (hotspot.GetComponent<HotspotScript>() == null)
            {
                hotspot.AddComponent<HotspotScript>();
            }

            panoramaSelectionUI.ShowSelectionMenu(hotspot);
        }
        else
        {
            Debug.LogWarning("Camera with tag '" + cameraTagName + "' not found.");
        }
    }
}
