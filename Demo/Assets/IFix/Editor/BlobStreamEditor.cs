using UnityEditor;
using UnityEngine;
using System.IO;

public class BlobStreamEditor
{
    const string PatchFilePath = "Assets/StreamingAssets/Main.patch.bytes";
    const string PrefabPath = "Assets/Resources/InjectFix/InjectFix.prefab";
    
    const string PatchFilePath1 = "Assets/StreamingAssets/Plugins.patch.bytes";
    const string PrefabPath1 = "Assets/Resources/InjectFix/InjectFix1.prefab";

    [MenuItem("InjectFix/Build Patch To Prefab", false, 5)]
    public static void BuildPatchToPrefab()
    {
        BuildPatchToPrefabInternal(PatchFilePath, PrefabPath, "InjectFix");
        BuildPatchToPrefabInternal(PatchFilePath1, PrefabPath1, "InjectFix1");

        AssetDatabase.Refresh();
    }

    static void BuildPatchToPrefabInternal(string patchFilePath, string prefabPath, string prefabName)
    {
        if (!File.Exists(patchFilePath))
        {
            Debug.LogError("Patch file not found: " + patchFilePath);
            return;
        }

        byte[] patchData = File.ReadAllBytes(patchFilePath);

        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            var go = new GameObject(prefabName);
            var blobStream = go.AddComponent<BloStream>();
            blobStream.SetData(patchData);

            Directory.CreateDirectory(Path.GetDirectoryName(prefabPath));
            PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            Object.DestroyImmediate(go);
            Debug.Log("Created prefab with patch data, size: " + patchData.Length);
        }
        else
        {
            var blobStream = prefab.GetComponent<BloStream>();
            if (blobStream == null)
            {
                Debug.LogError("BlobStream component not found on prefab: " + prefabPath);
                return;
            }

            var serializedObject = new SerializedObject(blobStream);
            var dataProp = serializedObject.FindProperty("_data");

            dataProp.arraySize = patchData.Length;
            for (int i = 0; i < patchData.Length; i++)
            {
                dataProp.GetArrayElementAtIndex(i).intValue = patchData[i];
            }

            var sizeProp = serializedObject.FindProperty("DataSize");
            sizeProp.longValue = patchData.Length;

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(prefab);
            AssetDatabase.SaveAssets();

            Debug.Log("Updated prefab patch data, size: " + patchData.Length);
        }
    }
}
