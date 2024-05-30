// using UnityEngine;
// using UnityEngine.UI;

// public class ImageSelector : MonoBehaviour
// {
//     public GameObject imageContainer; // Reference to the empty GameObject containing image child objects
//     public GameObject sphereButtonPrefab; // Prefab of the sphere button
//     public string canvasTag = "TempCanvas"; // Tag of the canvas object

//     private void Start()
//     {
//         // Find the canvas object with the specified tag
//         GameObject canvas = GameObject.FindGameObjectWithTag(canvasTag);
//         if (canvas == null)
//         {
//             Debug.LogError("Canvas component not found on object with tag '" + canvasTag + "'.");
//             return;
//         }

//         // Iterate through each child object of the image container
//         foreach (Transform child in imageContainer.transform)
//         {
//             // Check if the child object has a texture component (e.g., Texture2D)
//             Texture2D texture = child.GetComponent<Texture2D>();
//             if (texture != null)
//             {
//                 // Instantiate a sphere button prefab as a child of the canvas
//                 GameObject sphereButton = Instantiate(sphereButtonPrefab, canvas.transform);

//                 // Example: Apply the texture to the sphere button
//                 Renderer sphereRenderer = sphereButton.GetComponent<Renderer>();
//                 if (sphereRenderer != null)
//                 {
//                     Material material = new Material(sphereRenderer.material);
//                     material.mainTexture = texture;
//                     sphereRenderer.material = material;
//                 }

//                 // Add a button component to the sphere button
//                 Button buttonComponent = sphereButton.AddComponent<Button>();
//                 if (buttonComponent != null)
//                 {
//                     // Add a listener to the button component to handle click events
//                     buttonComponent.onClick.AddListener(() => OnSphereButtonClick(texture));
//                 }
//             }
//         }
//     }

//     private void OnSphereButtonClick(Texture2D texture)
//     {
//         // Example: Log the name or index of the selected texture
//         Debug.Log("Selected Texture: " + texture.name);
//     }
// }
