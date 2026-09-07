using UnityEngine;

public class Hello
{
    // 热更入口：由 LoadDll 在 Assembly.Load(Game.dll) 成功后反射调用。
    // 此时热更类型已存在，可安全地动态挂载热更 MonoBehaviour。
    public static void Run()
    {
        Debug.Log("Hello, HybridCLR");

        // 动态挂载热更 MonoBehaviour（不能在场景里静态挂，见 UITest 注释）
        var go = new GameObject("UITest");
        Object.DontDestroyOnLoad(go);
        go.AddComponent<UITest>();
        Debug.Log("[Hello] 已动态创建 UITest");
    }
}
