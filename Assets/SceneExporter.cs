using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ExportObjects : MonoBehaviour
{
    public string exportFileName = "ExportedSceneData.json";

    public void ExportSelectedObjects()
    {
        // Define the path for the exported data
        string exportPath = Path.Combine(Application.persistentDataPath, exportFileName);

        // Collect the objects to export
        List<ObjectData> objectDataList = new List<ObjectData>();

        // Collect images
        GameObject[] images = GameObject.FindGameObjectsWithTag("Image");
        foreach (GameObject image in images)
        {
            ObjectData data = new ObjectData(image);
            objectDataList.Add(data);
        }

        // Collect hotspots
        GameObject[] hotspots = GameObject.FindGameObjectsWithTag("Hotspot");
        foreach (GameObject hotspot in hotspots)
        {
            ObjectData data = new ObjectData(hotspot);
            objectDataList.Add(data);
        }

        // Collect dollhouse model (assuming it's tagged as "Dollhouse")
        GameObject dollhouse = GameObject.FindGameObjectWithTag("Dollhouse");
        if (dollhouse != null)
        {
            ObjectData data = new ObjectData(dollhouse);
            objectDataList.Add(data);
        }

        // Serialize the object data to JSON
        string json = JsonUtility.ToJson(new ObjectDataCollection(objectDataList), true);

        // Save the JSON to a file
        File.WriteAllText(exportPath, json);

        Debug.Log("Selected objects exported to " + exportPath);
    }
}

[System.Serializable]
public class ObjectData
{
    public string name;
    public string tag;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public ObjectData(GameObject obj)
    {
        name = obj.name;
        tag = obj.tag;
        position = obj.transform.position;
        rotation = obj.transform.rotation;
        scale = obj.transform.localScale;
    }
}

[System.Serializable]
public class ObjectDataCollection
{
    public List<ObjectData> objectDataList;

    public ObjectDataCollection(List<ObjectData> dataList)
    {
        objectDataList = dataList;
    }
}
