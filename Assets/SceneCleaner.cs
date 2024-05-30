using UnityEngine;
using System.Collections.Generic;

public class SceneCleaner : MonoBehaviour
{
    [Header("Objects to Remove")]

    public List<GameObject> objectsToRemove = new List<GameObject>();
    

    public void CleanScene()
    {

        // Find all objects in the scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        // Create a set of objects to remove for faster lookup
        HashSet<GameObject> objectsToRemoveSet = new HashSet<GameObject>(objectsToRemove);

        // Remove specific objects
        foreach (GameObject obj in allObjects)
        {
            if (objectsToRemoveSet.Contains(obj))
            {
                DestroyImmediate(obj);
            }
        }

        Debug.Log("Scene cleaned. Specified objects removed.");
    }
}
