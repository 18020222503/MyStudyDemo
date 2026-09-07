using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 热更 DLL Bundle 的命名常量，打包端（BuildDllBundle）与运行时端（DllBundleLoader）共用，
/// 避免两边各写魔法字符串对不上。
/// </summary>
public static class DllBundleConst
{
    // AssetBundle 文件名（小写；原生 AssetBundle 名默认小写）
    public const string BundleName = "dll";

    // bundle 内 Game.dll 的 TextAsset 资产路径（用于 LoadAsset）。
    // 原生 AssetBundle 用完整 asset path 作为 name，打包时会 Log 实际值，如不一致以实际为准。
    public const string DllAssetPath = "assets/dlls/game.dll.bytes";

    // Assembly.Load 用的逻辑名（仅日志/标识用）
    public const string HotUpdateDll = "Game.dll";

    // 版本清单文件名
    public const string VersionFile = "version.json";

    // 本地缓存子目录（位于 persistentDataPath 下）
    public const string CacheDir = "dllcache";

    // ---- 平台子目录名 ----
    // 打包端与运行时端必须对同一平台返回相同字符串，才能保证下载路径 = 打包输出路径。
    // 统一采用 BuildTarget 的名称（如 "Android"、"StandaloneWindows64"、"iOS"）。

#if UNITY_EDITOR
    /// <summary>打包端：由构建目标得到平台子目录名。</summary>
    public static string GetPlatformName(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.Android: return "Android";
            case BuildTarget.iOS: return "iOS";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64: return "StandaloneWindows64";
            case BuildTarget.StandaloneOSX: return "StandaloneOSX";
            default: return target.ToString();
        }
    }
#endif

    /// <summary>运行时：由当前运行平台得到平台子目录名（须与 GetPlatformName 对齐）。</summary>
    public static string GetRuntimePlatformName()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android: return "Android";
            case RuntimePlatform.IPhonePlayer: return "iOS";
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor: return "StandaloneWindows64";
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.OSXEditor: return "StandaloneOSX";
            default: return Application.platform.ToString();
        }
    }
}
