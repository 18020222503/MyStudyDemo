using UnityEngine;
using UnityEngine.AddressableAssets;

public class ResourceManager:MonoBehaviour
{
    public AssetReference spherePrefabRef;
    private void Start()
    {
        LoadResources();
        // LoadReferenceRes();
    }

    // 加载
    public void LoadResources()
    {
        Addressables.LoadAssetAsync<GameObject>("HelloCube").Completed += (handle) =>
            {
                // 预设物体
                GameObject prefabObj = handle.Result;
                GameObject cubeObj = Instantiate(prefabObj);
            };
    }


    public void LoadReferenceRes()
    {
        spherePrefabRef.LoadAssetAsync<GameObject>().Completed += (obj) =>
            {
                // 预设
                GameObject spherePrefab = obj.Result;
                // 实例化
                GameObject sphereObj = Instantiate(spherePrefab);
            };
    }
}
