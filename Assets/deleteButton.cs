// using UnityEngine;
// using UnityEngine.UI;

// public class DeleteImageButton : MonoBehaviour
// {
//   public ImageUploader imageUploader; // Reference to the ImageUploader script

//   private void Start()
//   {
//     Button button = GetComponent<Button>(); // Get the Button component
//     button.onClick.AddListener(OnDeleteButtonClick); // Add listener for click event
//   }

//   private void OnDeleteButtonClick()
//   {
//     if (imageUploader != null)
//     {
//       // Get the RawImage associated with this button (assuming it's a child or sibling)
//       RawImage imageToDelete = transform.parent.GetComponentInChildren<RawImage>();
//       if (imageToDelete != null)
//       {
//         imageUploader.DeleteImage(imageToDelete); // Call the DeleteImage function in ImageUploader
//       }
//     }
//     else
//     {
//       Debug.LogError("DeleteImageButton: Missing reference to ImageUploader script!");
//     }
//   }
// }
