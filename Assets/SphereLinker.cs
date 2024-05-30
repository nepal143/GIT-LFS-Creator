// using UnityEngine;

// public class SphereLinker : MonoBehaviour
// {
//     private GameObject selectedSphere; 

//     void Update()
//     {
        
//         if (Input.GetMouseButtonDown(0))
//         {
//             Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//             RaycastHit hit;

//             if (Physics.Raycast(ray, out hit))
//             {
//                 GameObject sphere = hit.collider.gameObject;
//                 if (sphere.CompareTag("SpherePrefab"))
//                 {
//                     if (selectedSphere != null)
//                     {
//                         LinkSpheres(selectedSphere, sphere);
//                         selectedSphere = null; 
//                     }
//                     else
//                     {
//                         selectedSphere = sphere; 
//                     }
//                 }
//             }
//         }
//     }

//     void LinkSpheres(GameObject sphere1, GameObject sphere2)
//     {
//         Debug.Log("Linked sphere " + sphere1.name + " to " + sphere2.name);
//     }
// }
