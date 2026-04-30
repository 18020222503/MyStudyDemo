using System.Collections;
using System.IO;
using IFix;
using UnityEngine;
using IFix.Core;

public class InjectFixController : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return LoadPatchFromPrefab("InjectFix/InjectFix");
        yield return LoadPatchFromPrefab("InjectFix/InjectFix1");
    }

    IEnumerator LoadPatchFromPrefab(string prefabPath)
    {
        var prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab != null)
        {
            var blobStream = prefab.GetComponent<BloStream>();
            if (blobStream != null && blobStream.GetData() != null && blobStream.GetData().Length > 0)
            {
                Debug.Log("Loading patch from " + prefabPath + ", size: " + blobStream.DataSize);
                PatchManager.Load(new MemoryStream(blobStream.GetData()), true);
                Debug.Log("Patch loaded successfully from " + prefabPath);
                yield break;
            }
        }

        // string fileName = Path.GetFileNameWithoutExtension(prefabPath) + ".patch.bytes";
        // string path = Path.Combine(Application.streamingAssetsPath, fileName).Replace("\\", "/");
        // Debug.Log("Patch path: " + path);
        // using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(path))
        // {
        //     yield return request.SendWebRequest();
        //     if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
        //     {
        //         Debug.Log("Loading patch from " + path);
        //         PatchManager.Load(new MemoryStream(request.downloadHandler.data), true);
        //         Debug.Log("Patch loaded successfully from " + path);
        //     }
        //     else
        //     {
        //         Debug.LogError("Patch load failed: " + request.error);
        //     }
        // }
    }

    [Patch]
    public void Print()
    {
        Debug.Log("【InjectFixController】 Print#22222#####################*");
    }
}
