using UnityEngine;
using UnityEngine.UI;

public class DeleteButtonScript : MonoBehaviour
{
    void Start()
    {
        // Add listener to the delete button
        GetComponent<Button>().onClick.AddListener(OnDeleteButtonClick);
    }

    public void OnDeleteButtonClick()
    {
        Debug.Log("Delete button clicked!");

        // Get the parent of the DeleteButton, which is the RawImage container
        GameObject imageContainer = transform.parent.gameObject;

        // Find the ImageUploader script in the scene
        ImageUploader imageUploader = FindObjectOfType<ImageUploader>();

        // Check if the ImageUploader script was found
        if (imageUploader != null)
        {
            // Call the DeleteImage method from the ImageUploader script
            imageUploader.DeleteImage(imageContainer);

            Debug.Log("DeleteImage method called on ImageUploader.");
        }
        else
        {
            Debug.LogError("ImageUploader script not found in the scene.");
        }
    }
}
