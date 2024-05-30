using UnityEngine;

public class PanoramaTeleport : MonoBehaviour
{
    public int panoramaDistance = 50; 
    private int currentPanoramaIndex = 0; 

    private void Update()
    {
      
        if (Input.GetKeyDown(KeyCode.Tab))
        {
           
            TeleportToNextPanorama();
        }
    }

    private void TeleportToNextPanorama()
{
   
    if (transform.childCount == 0)
    {
        Debug.LogWarning("No panoramas found as children.");
        return;
    }

   
    currentPanoramaIndex++;

   
    if (currentPanoramaIndex >= transform.childCount)
    {
        currentPanoramaIndex = 0;
    }

 
    Transform nextPanoramaTransform = transform.GetChild(currentPanoramaIndex);

   
    transform.position = nextPanoramaTransform.position;
}

}
