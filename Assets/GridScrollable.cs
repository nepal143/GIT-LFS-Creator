// using UnityEngine;
// using UnityEngine.UI;

// public class DynamicScrollableGridLayout : MonoBehaviour
// {
//     public GameObject prefabItem; // Prefab of the image container
//     public GridLayoutGroup gridLayout; // Reference to the GridLayoutGroup component

//     void Start()
//     {
//         // Set up the grid layout
//         gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
//         gridLayout.constraintCount = 1; // Change this value based on your desired layout

//         // Example: Call a method to update the grid layout with new items
//         UpdateGridItems(20); // Initial number of items

//         // Example: Call the same method with a different number of items later
//         UpdateGridItems(30); // Updated number of items
//     }

//     // Method to update the grid layout with new image containers
//     public void UpdateGridItems(int numberOfItems)
//     {
//         // Clear existing items
//         foreach (Transform child in transform)
//         {
//             Destroy(child.gameObject);
//         }

//         // Create new items in the grid
//         for (int i = 0; i < numberOfItems; i++)
//         {
//             GameObject item = Instantiate(prefabItem, transform);
//             // Customize the item here (e.g., set image, color, etc.)

//             // Set the size of the image container
//             RectTransform rectTransform = item.GetComponent<RectTransform>();
//             rectTransform.sizeDelta = new Vector2(100f, 100f);
//         }

//         // Adjust the cell size and spacing of the grid layout
//         gridLayout.cellSize = new Vector2(100f, 100f); // Set cell size to match item size
//         gridLayout.spacing = new Vector2(10f, 10f); // Set spacing (margin) between items
//     }
// }
