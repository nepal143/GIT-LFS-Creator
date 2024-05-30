using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float rotationSpeed = 5f;

    private void Update()
    {
     
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

       
        transform.Rotate(Vector3.up, mouseX, Space.World);

       
        transform.Rotate(Vector3.left, mouseY, Space.Self);
    }
}
