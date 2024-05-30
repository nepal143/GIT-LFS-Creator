using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PanoramaSelectionUI : MonoBehaviour
{
    public GameObject imageButtonPrefab; // Prefab for panorama image buttons
    public Transform buttonContainer; // Container for the buttons (should be the Content GameObject of ScrollView)
    public ImageUploader imageUploader; // Reference to the ImageUploader script
    private GameObject currentHotspot; // The current hotspot being configured

    private void Start()
    {
        // Initially hide the panel
        gameObject.SetActive(false);
    }

    public void ShowSelectionMenu(GameObject hotspot)
    {
        Debug.Log("ShowSelectionMenu called for hotspot: " + hotspot.name);

        // Set the current hotspot
        currentHotspot = hotspot;

        // Remove existing buttons
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // Check how many textures are in the imageUploader
        Debug.Log("Number of uploaded textures: " + imageUploader.uploadedTextures.Count);

        // Create an image button for each uploaded panorama
        for (int i = 0; i < imageUploader.uploadedTextures.Count; i++)
        {
            Debug.Log("Creating button for texture index: " + i);

            GameObject buttonObj = Instantiate(imageButtonPrefab, buttonContainer);
            Button button = buttonObj.GetComponent<Button>();
            int index = i; // Capture the current index for the lambda
            button.onClick.AddListener(() => OnPanoramaButtonClick(index));

            // Set the button image to represent the panorama
            RawImage buttonImage = button.GetComponentInChildren<RawImage>();
            if (buttonImage != null)
            {
                buttonImage.texture = imageUploader.uploadedTextures[index];
                Debug.Log("Button image set for texture index: " + index);
            }
            else
            {
                Debug.LogError("RawImage component not found in children of imageButtonPrefab.");
            }
        }

        // Show the panel
        gameObject.SetActive(true);
        Debug.Log("Selection menu shown.");
    }

    private void OnPanoramaButtonClick(int index)
    {
        Debug.Log("Panorama button clicked: " + index);

        // Connect the hotspot to the selected panorama
        ConnectHotspotToPanorama(index);

        // Hide the panel
        gameObject.SetActive(false);
    }

    private void ConnectHotspotToPanorama(int panoramaIndex)
    {
        Debug.Log("Connecting hotspot to panorama: " + panoramaIndex);

        HotspotScript hotspotScript = currentHotspot.GetComponent<HotspotScript>();
        if (hotspotScript != null)
        {
            hotspotScript.connectedPanoramaIndex = panoramaIndex;
        }
        else
        {
            Debug.LogError("HotspotScript not found on the hotspot.");
        }
    }
}
