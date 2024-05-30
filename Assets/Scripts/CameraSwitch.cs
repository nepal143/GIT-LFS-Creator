// using UnityEngine;

// public class CameraSwitch : MonoBehaviour
// {
//     public Camera firstPersonCamera;
//     public Camera thirdPersonCamera;
//     public MonoBehaviour firstPersonScript;
//     public MonoBehaviour thirdPersonScript;

//     void Start()
//     {
//         // Ensure one camera is enabled and the other is disabled at the start
//         firstPersonCamera.enabled = true;
//         thirdPersonCamera.enabled = false;
        
//         // Ensure the corresponding scripts are enabled/disabled accordingly
//         firstPersonScript.enabled = true;
//         thirdPersonScript.enabled = false;
//     }

//     void Update()
//     {
//         // Switch cameras when the spacebar is pressed
//         if (Input.GetKeyDown(KeyCode.Space))
//         {
//             // Toggle camera and corresponding script
//             firstPersonCamera.enabled = !firstPersonCamera.enabled;
//             thirdPersonCamera.enabled = !thirdPersonCamera.enabled;
//             firstPersonScript.enabled = !firstPersonScript.enabled;
//             thirdPersonScript.enabled = !thirdPersonScript.enabled;
//         }
//     }
// }
