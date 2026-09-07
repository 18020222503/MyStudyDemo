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
}
