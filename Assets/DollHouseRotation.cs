using UnityEngine;
using UnityEngine.UI;

public class DollHouseRotationController : MonoBehaviour
{
    public string dollHouseTag = "DollHouse"; // Tag to identify the dollhouse
    public float rotationSpeed = 10f; // Speed of rotation

    // Rotation buttons
    public Button rotateLeftButton;         // Rotate around Y axis (Left)
    public Button rotateRightButton;        // Rotate around Y axis (Right)
    public Button rotateUpButton;           // Rotate around X axis (Up)
    public Button rotateDownButton;         // Rotate around X axis (Down)
    public Button rotateClockwiseButton;    // Rotate around Z axis (Clockwise)
    public Button rotateCounterClockwiseButton; // Rotate around Z axis (Counter-Clockwise)

    private GameObject dollHouse;

    void Start()
    {
        FindDollHouse();

        // Assign the rotation functions to the buttons
        if (rotateLeftButton != null)
            rotateLeftButton.onClick.AddListener(RotateLeft);

        if (rotateRightButton != null)
            rotateRightButton.onClick.AddListener(RotateRight);

        if (rotateUpButton != null)
            rotateUpButton.onClick.AddListener(RotateUp);

        if (rotateDownButton != null)
            rotateDownButton.onClick.AddListener(RotateDown);

        if (rotateClockwiseButton != null)
            rotateClockwiseButton.onClick.AddListener(RotateClockwise);

        if (rotateCounterClockwiseButton != null)
            rotateCounterClockwiseButton.onClick.AddListener(RotateCounterClockwise);
    }

    void FindDollHouse()
    {
        dollHouse = GameObject.FindGameObjectWithTag(dollHouseTag);

        if (dollHouse == null)
        {
            Debug.LogError($"Dollhouse with tag {dollHouseTag} not found. Make sure it is tagged correctly.");
        }
        else
        {
            Debug.Log($"Dollhouse found and assigned.");
        }
    }

    void RotateLeft()
    {
        RotateDollHouse(Vector3.up*90f);
    }

    void RotateRight()
    {
        RotateDollHouse(Vector3.down*90f);
    }

    void RotateUp()
    {
        RotateDollHouse(Vector3.right*90f);
    }

    void RotateDown()
    {
        RotateDollHouse(Vector3.left*90f);
    }

    void RotateClockwise()
    {
        RotateDollHouse(Vector3.forward*90f);
    }

    void RotateCounterClockwise()
    {
        RotateDollHouse(Vector3.back*90f);
    }

    void RotateDollHouse(Vector3 direction)
    {
        if (dollHouse != null)
        {
            dollHouse.transform.Rotate(direction * rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}
