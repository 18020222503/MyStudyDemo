using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    // Addressables 已从项目移除。原先通过 Addressables 加载资源的逻辑先行下线，
    // 后续如需资源加载请改用 Resources / AssetBundle 或新的资源方案。
    private void Start()
    {
        LoadResources();
    }

    // 加载
    public void LoadResources()
    {
        // TODO: 替换为新的资源加载方案（原实现使用 Addressables.LoadAssetAsync("HelloCube")）
        var prefab = Resources.Load<GameObject>("HelloCube");
        if (prefab != null)
        {
            Instantiate(prefab);
        }
    }
}
