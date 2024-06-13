using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.IO.Compression; // For ZipArchive
using System.Runtime.Serialization.Formatters.Binary;

namespace SceneStateExport
{
    public class SceneExporter : MonoBehaviour
    {
        public GameObject rootObject;
        public string exportFolderPath = "MyExports";
        public string exportFileName = "ExportedScene.zip";

        public void ExportRootObject()
        {
            if (rootObject == null)
            {
                Debug.LogError("Root object not assigned.");
                return;
            }

            // Ensure the export directory exists
            string fullExportFolderPath = Path.Combine(Application.persistentDataPath, exportFolderPath);
            if (!Directory.Exists(fullExportFolderPath))
            {
                Directory.CreateDirectory(fullExportFolderPath);
            }

            // Find the DollHouse object by its tag
            GameObject dollHouseObject = GameObject.FindGameObjectWithTag("DollHouse");
            if (dollHouseObject == null)
            {
                Debug.LogError("DollHouse object with tag 'DollHouse' not found.");
                return;
            }

            // Save runtime assets (meshes and materials) for the DollHouse
            SaveRuntimeAssets(dollHouseObject);

            // Save the DollHouse object as a prefab with a specific name
            string dollHousePrefabPath = SaveObjectAsPrefab(dollHouseObject, "DollHouse");

            if (string.IsNullOrEmpty(dollHousePrefabPath))
            {
                Debug.LogError("Failed to save prefab for DollHouse.");
                return;
            }

            // Parent the DollHouse object to the rootObject if it's not already
            if (dollHouseObject.transform.parent != rootObject.transform)
            {
                dollHouseObject.transform.SetParent(rootObject.transform, true);
            }

            // Find all game objects in the scene
            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

            // Parent all objects outside the rootObject to it
            foreach (GameObject obj in allObjects)
            {
                if (obj != rootObject && obj.transform.parent == null && obj != dollHouseObject)
                {
                    obj.transform.SetParent(rootObject.transform);
                }
            }

            // Save runtime assets (meshes and materials) for all objects
            SaveRuntimeAssets(rootObject);

            // Save the root object as a prefab
            string rootPrefabPath = SaveObjectAsPrefab(rootObject, rootObject.name);

            if (string.IsNullOrEmpty(rootPrefabPath))
            {
                Debug.LogError("Failed to save prefab for root object.");
                return;
            }

            // Export the prefabs to a package
            string packagePath = Path.Combine(fullExportFolderPath, exportFileName);
            CreateZipPackage(packagePath, new string[] { rootPrefabPath, dollHousePrefabPath });

            Debug.Log("Root object and DollHouse exported to " + packagePath);
        }

        private string SaveObjectAsPrefab(GameObject obj, string prefabName)
        {
            string path = Path.Combine(Application.persistentDataPath, exportFolderPath, prefabName + ".prefab");
            // Here you can save the object in a format you prefer, using JSON or other methods.
            // As an example, we'll just create an empty file to represent the prefab.
            File.WriteAllText(path, "Prefab data for " + prefabName);
            return path;
        }

        private void SaveRuntimeAssets(GameObject obj)
        {
            foreach (MeshFilter meshFilter in obj.GetComponentsInChildren<MeshFilter>())
            {
                SaveMeshAsset(meshFilter.mesh, meshFilter.gameObject.name);
            }

            foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>())
            {
                SaveMaterialAssets(renderer.sharedMaterials, renderer.gameObject.name);
            }
        }

        private void SaveMeshAsset(Mesh mesh, string objName)
        {
            if (mesh == null) return;

            string path = Path.Combine(Application.persistentDataPath, exportFolderPath, objName + "_Mesh.asset");
            // Serialize the mesh data to a file
            byte[] meshData = MeshToBytes(mesh);
            File.WriteAllBytes(path, meshData);
        }

        private byte[] MeshToBytes(Mesh mesh)
        {
            // Serialize the mesh to a byte array
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, mesh);
                return stream.ToArray();
            }
        }

        private void SaveMaterialAssets(Material[] materials, string objName)
        {
            foreach (Material material in materials)
            {
                if (material == null || material.name == "Default-Material") continue; // Skip default material

                string path = Path.Combine(Application.persistentDataPath, exportFolderPath, objName + "_Material_" + material.name + ".mat");
                // Serialize the material data to a file
                byte[] materialData = MaterialToBytes(material);
                File.WriteAllBytes(path, materialData);

                if (material.mainTexture != null)
                {
                    string texturePath = Path.Combine(Application.persistentDataPath, exportFolderPath, objName + "_Texture_" + material.mainTexture.name + ".asset");
                    byte[] textureData = TextureToBytes((Texture2D)material.mainTexture);
                    File.WriteAllBytes(texturePath, textureData);
                }
            }
        }

        private byte[] MaterialToBytes(Material material)
        {
            // Serialize the material to a byte array
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, material);
                return stream.ToArray();
            }
        }

        private byte[] TextureToBytes(Texture2D texture)
        {
            return texture.EncodeToPNG();
        }

        private void CreateZipPackage(string zipPath, string[] filePaths)
        {
            using (FileStream zipToOpen = new FileStream(zipPath, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                {
                    foreach (string filePath in filePaths)
                    {
                        archive.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
                    }
                }
            }
        }
    }
}
