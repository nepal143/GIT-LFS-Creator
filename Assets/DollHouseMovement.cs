using UnityEngine;
using UnityEngine.UI;

public class DollHouseMovementController : MonoBehaviour
{
    public string dollHouseTag = "DollHouse"; // Tag to identify the dollhouse
    public float movementSpeed = 1f; // Speed of movement

    public Button moveLeftButton;
    public Button moveRightButton;
    public Button moveUpButton;
    public Button moveDownButton;
    public Button moveForwardButton;
    public Button moveBackwardButton;

    private GameObject dollHouse;

    void Start()
    {
        FindDollHouse();

        // Assign the movement functions to the buttons
        if (moveLeftButton != null)
            moveLeftButton.onClick.AddListener(MoveLeft);

        if (moveRightButton != null)
            moveRightButton.onClick.AddListener(MoveRight);

        if (moveUpButton != null)
            moveUpButton.onClick.AddListener(MoveUp);

        if (moveDownButton != null)
            moveDownButton.onClick.AddListener(MoveDown);

        if (moveForwardButton != null)
            moveForwardButton.onClick.AddListener(MoveForward);

        if (moveBackwardButton != null)
            moveBackwardButton.onClick.AddListener(MoveBackward);
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

    void MoveLeft()
    {
        MoveDollHouse(Vector3.left);
    }

    void MoveRight()
    {
        MoveDollHouse(Vector3.right);
    }

    void MoveUp()
    {
        MoveDollHouse(Vector3.up);
    }

    void MoveDown()
    {
        MoveDollHouse(Vector3.down);
    }

    void MoveForward()
    {
        MoveDollHouse(Vector3.forward);
    }

    void MoveBackward()
    {
        MoveDollHouse(Vector3.back);
    }

    void MoveDollHouse(Vector3 direction)
    {
        if (dollHouse != null)
        {
            dollHouse.transform.Translate(direction * movementSpeed * Time.deltaTime, Space.World);
        }
    }
}
