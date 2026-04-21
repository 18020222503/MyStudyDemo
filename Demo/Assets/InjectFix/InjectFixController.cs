using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using IFix;
using IFix.Core;

public class InjectFixController : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Assembly-CSharp.patch.bytes").Replace("\\", "/");
        Debug.Log("Patch path: " + path);
        using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(path))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.Log("Loading patch...");
                PatchManager.Load(new MemoryStream(request.downloadHandler.data), true);
                Debug.Log("Patch loaded successfully!");
            }
            else
            {
                Debug.LogError("Patch load failed: " + request.error);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
