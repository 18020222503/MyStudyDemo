using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class LoadDll : MonoBehaviour
{
    void Start()
    {
#if UNITY_EDITOR
        // Editor 下 Game 程序集已随工程编译加载，直接查找，无需热更下载
        Assembly hotUpdateAss = AppDomain.CurrentDomain.GetAssemblies()
            .First(a => a.GetName().Name == "Game");
        InvokeEntry(hotUpdateAss);
#else
        // 真机：从 CDN 下载 Game.dll 的 AssetBundle 并加载，完成后反射进入热更入口
        var loader = gameObject.AddComponent<DllBundleLoader>();
        loader.Load(hotUpdateAss =>
        {
            if (hotUpdateAss == null)
            {
                Debug.LogError("[LoadDll] 热更程序集加载失败");
                return;
            }
            InvokeEntry(hotUpdateAss);
        });
#endif
    }

    // 反射调用热更入口 Hello.Run()
    private static void InvokeEntry(Assembly hotUpdateAss)
    {
        Type type = hotUpdateAss.GetType("Hello");
        if (type == null)
        {
            Debug.LogError("[LoadDll] 未找到 Hello 类型");
            return;
        }
        type.GetMethod("Run")?.Invoke(null, null);
    }
}
