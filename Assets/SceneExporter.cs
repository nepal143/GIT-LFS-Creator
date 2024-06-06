using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace SceneStateExport
{
    public class SceneExporter : MonoBehaviour
    {
        public GameObject rootObject;
        public string exportFolderPath = "Assets/MyExports";
        public string exportFileName = "ExportedScene.unitypackage";

        public void ExportRootObject()
        {
            if (rootObject == null)
            {
                Debug.LogError("Root object not assigned.");
                return;
            }

            // Ensure the export directory exists
            if (!AssetDatabase.IsValidFolder(exportFolderPath))
            {
                AssetDatabase.CreateFolder("Assets", "MyExports");
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
            string packagePath = Path.Combine(exportFolderPath, exportFileName);
            AssetDatabase.ExportPackage(new string[] { rootPrefabPath, dollHousePrefabPath }, packagePath, ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Recurse);

            Debug.Log("Root object and DollHouse exported to " + packagePath);
        }

        private string SaveObjectAsPrefab(GameObject obj, string prefabName)
        {
            string path = Path.Combine(exportFolderPath, prefabName + ".prefab");
            Object prefab = PrefabUtility.SaveAsPrefabAsset(obj, path);

            if (prefab != null)
            {
                return path;
            }
            else
            {
                Debug.LogError("Failed to save prefab for " + prefabName);
                return null;
            }
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

            string path = Path.Combine(exportFolderPath, objName + "_Mesh.asset");

            // Check if the asset already exists
            Mesh existingMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (existingMesh == null)
            {
                AssetDatabase.CreateAsset(mesh, path);
            }
            else
            {
                Debug.Log("Mesh asset already exists: " + path);
            }
        }

        private void SaveMaterialAssets(Material[] materials, string objName)
        {
            foreach (Material material in materials)
            {
                if (material == null || material.name == "Default-Material") continue; // Skip default material

                // Create a copy of the material to avoid modifying the original shared material
                Material newMaterial = new Material(material);

                // Save the texture if it's not already an asset
                if (newMaterial.mainTexture != null && !AssetDatabase.Contains(newMaterial.mainTexture))
                {
                    string texturePath = Path.Combine(exportFolderPath, objName + "_Texture_" + newMaterial.mainTexture.name + ".asset");
                    Texture existingTexture = AssetDatabase.LoadAssetAtPath<Texture>(texturePath);
                    if (existingTexture == null)
                    {
                        AssetDatabase.CreateAsset(newMaterial.mainTexture, texturePath);
                        newMaterial.mainTexture = AssetDatabase.LoadAssetAtPath<Texture>(texturePath);
                    }
                    else
                    {
                        Debug.Log("Texture asset already exists: " + texturePath);
                        newMaterial.mainTexture = existingTexture;
                    }
                }

                // Create asset for the new material
                string materialPath = Path.Combine(exportFolderPath, objName + "_Material_" + newMaterial.name + ".mat");
                Material existingMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                if (existingMaterial == null)
                {
                    AssetDatabase.CreateAsset(newMaterial, materialPath);
                }
                else
                {
                    Debug.Log("Material asset already exists: " + materialPath);
                }
            }
        }
    }
}
