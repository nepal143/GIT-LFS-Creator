// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections.Generic;

// public class ImageDisplay : MonoBehaviour
// {
//     public GameObject imagePrefab;
//     public Transform gridContainer;
//     public float gapBetweenImages = 10f;
//     public ScrollRect scrollRect;

//     private List<Texture2D> uploadedTextures = new List<Texture2D>();

//     private void Start()
//     {
//         // Example: Manually add some textures for testing
//         for (int i = 1; i <= 5; i++)
//         {
//             Texture2D texture = Resources.Load<Texture2D>("Textures/Example" + i);
//             if (texture != null)
//             {
//                 uploadedTextures.Add(texture);
//             }
//         }

//         // Create image planes for each uploaded image
//         DisplayImages();
//     }

//     private void DisplayImages()
//     {
//         // Clear existing image planes
//         foreach (Transform child in gridContainer)
//         {
//             Destroy(child.gameObject);
//         }

//         // Instantiate image planes for each uploaded texture
//         float currentX = 0f;
//         float currentY = 0f;

//         for (int i = 0; i < uploadedTextures.Count; i++)
//         {
//             GameObject imageObject = Instantiate(imagePrefab, gridContainer);
//             imageObject.transform.localPosition = new Vector3(currentX, -currentY, 0f);

//             RawImage rawImage = imageObject.GetComponent<RawImage>();
//             if (rawImage != null)
//             {
//                 rawImage.texture = uploadedTextures[i];
//             }

//             currentX += imageObject.GetComponent<RectTransform>().rect.width + gapBetweenImages;

//             // Check if the next image will go out of bounds
//             if (currentX + imageObject.GetComponent<RectTransform>().rect.width > gridContainer.GetComponent<RectTransform>().rect.width)
//             {
//                 currentX = 0f;
//                 currentY += imageObject.GetComponent<RectTransform>().rect.height + gapBetweenImages;
//             }
//         }

//         // Resize the grid container to fit the images
//         RectTransform gridRectTransform = gridContainer.GetComponent<RectTransform>();
//         if (gridRectTransform != null)
//         {
//             gridRectTransform.sizeDelta = new Vector2(gridRectTransform.sizeDelta.x, currentY + imagePrefab.GetComponent<RectTransform>().rect.height);
//         }

//         // Enable the scroll view if needed
//         if (scrollRect != null && gridRectTransform != null && gridRectTransform.rect.height > scrollRect.viewport.rect.height)
//         {
//             scrollRect.gameObject.SetActive(true);
//         }
//     }
// }
