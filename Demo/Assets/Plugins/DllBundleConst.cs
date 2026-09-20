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
    public const string BundleName = "dll";
    public const string DllAssetPath = "assets/dlls/game.dll.bytes";
    public const string HotUpdateDll = "Game.dll";

    public const string VersionFile = "version.json";
    public const string CacheDir = "dllcache";



#if UNITY_EDITOR
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
