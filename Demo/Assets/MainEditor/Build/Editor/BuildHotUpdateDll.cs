using System.IO;
using UnityEditor;
using UnityEngine;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;

/// <summary>
/// 编译 HybridCLR 热更程序集（Game.dll 等），拷贝成 *.dll.bytes 到 Assets/Dlls。
/// .bytes 后缀是为了让 Unity 把 DLL 当作 TextAsset 导入，便于打包 / 运行时 Assembly.Load。
/// </summary>
public static class BuildHotUpdateDll
{
    // 拷贝目标目录（相对工程根）
    private const string DstDir = "Assets/Dlls";

    [MenuItem("BuildPackage/编译并拷贝热更DLL(.bytes)", false, 20)]
    public static void CompileAndCopy()
    {
        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;

        // 1. 编译热更程序集
        CompileDllCommand.CompileDll(target, false);
        Debug.Log($"[BuildHotUpdateDll] CompileDll done. target={target}");

        // 2. 拷贝 .dll -> .dll.bytes
        int count = CopyHotUpdateDlls(target);

        AssetDatabase.Refresh();
        Debug.Log($"[BuildHotUpdateDll] 完成，共拷贝 {count} 个 DLL 到 {DstDir}");
    }

    /// <summary>
    /// 只拷贝（不编译）。前提是已经编译过热更 DLL。
    /// </summary>
    [MenuItem("BuildPackage/仅拷贝热更DLL(.bytes)", false, 21)]
    public static void CopyOnly()
    {
        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
        int count = CopyHotUpdateDlls(target);
        AssetDatabase.Refresh();
        Debug.Log($"[BuildHotUpdateDll] 仅拷贝完成，共 {count} 个 DLL 到 {DstDir}");
    }

    private static int CopyHotUpdateDlls(BuildTarget target)
    {
        string srcDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
        if (!Directory.Exists(srcDir))
        {
            Debug.LogError($"[BuildHotUpdateDll] 热更 DLL 输出目录不存在：{srcDir}，请先编译热更程序集。");
            return 0;
        }

        Directory.CreateDirectory(DstDir);

        int count = 0;
        // 遍历所有热更程序集（如 Game.dll），逐个拷成 .bytes
        foreach (string dllName in SettingsUtil.HotUpdateAssemblyFilesExcludePreserved)
        {
            string srcPath = Path.Combine(srcDir, dllName);
            if (!File.Exists(srcPath))
            {
                Debug.LogError($"[BuildHotUpdateDll] 找不到热更 DLL：{srcPath}");
                continue;
            }

            string dstPath = Path.Combine(DstDir, dllName + ".bytes");
            // 用字节拷贝而非 File.Copy，避免源文件被占用时的锁问题，也语义更明确
            File.WriteAllBytes(dstPath, File.ReadAllBytes(srcPath));
            count++;
            Debug.Log($"[BuildHotUpdateDll] {srcPath} -> {dstPath}");
        }
        return count;
    }
}
