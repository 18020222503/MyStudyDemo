using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using HybridCLR.Editor.Commands;

/// <summary>
/// 把 Assets/Dlls/Game.dll.bytes 打成一个 AssetBundle，输出到工程外的 BuildBundles/&lt;平台&gt;/，
/// 并生成版本清单 version.json（bundle 名 + md5 + size），供 CDN 下发。
/// </summary>
public static class BuildDllBundle
{
    // 源 .bytes（由 BuildHotUpdateDll 生成）
    private const string SrcBytes = "Assets/Dlls/Game.dll.bytes";
    // bundle 输出根目录（工程外，避免被 Unity 当资源导入）
    private static string OutputRoot => @"E:\study\Demo\Bundle";

    /// <summary>
    /// 一键：HybridCLR Generate/All → 编译并拷贝热更DLL(.bytes) → 打 Bundle。
    /// 整合原来需要手点的三步，按顺序执行，任一步抛异常即中止并报错。
    /// </summary>
    [MenuItem("BuildPackage/一键: 生成+编译+打Bundle", false, 10)]
    public static void GenerateCompileAndBuild()
    {
        try
        {
            // 步骤1：HybridCLR 生成（AOT 泛型引用 / 方法桥接 / link.xml 等，针对当前 activeBuildTarget）
            Debug.Log("[一键] 步骤1/3 HybridCLR Generate/All ...");
            PrebuildCommand.GenerateAll();

            // 步骤2：编译热更程序集并拷成 Assets/Dlls/*.dll.bytes
            Debug.Log("[一键] 步骤2/3 编译并拷贝热更DLL(.bytes) ...");
            BuildHotUpdateDll.CompileAndCopy();

            // 步骤3：打 Bundle + 生成 version.json（输出到 CDN 目录）
            Debug.Log("[一键] 步骤3/3 打包热更DLL Bundle ...");
            Build();

            Debug.Log("[一键] 全部完成 ✓");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[一键] 失败：{e}");
        }
    }

    [MenuItem("BuildPackage/打包热更DLL Bundle", false, 30)]
    public static void Build()
    {
        // 1. 确保 Game.dll.bytes 存在；没有就先编译+拷贝
        if (!File.Exists(SrcBytes))
        {
            Debug.Log($"[BuildDllBundle] {SrcBytes} 不存在，先执行编译+拷贝");
            BuildHotUpdateDll.CompileAndCopy();
        }
        if (!File.Exists(SrcBytes))
        {
            Debug.LogError($"[BuildDllBundle] 仍找不到 {SrcBytes}，中止打包");
            return;
        }

        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
        string outDir = Path.Combine(OutputRoot, DllBundleConst.GetPlatformName(target));
        Directory.CreateDirectory(outDir);

        // 2. 显式构建单个 bundle
        var build = new AssetBundleBuild
        {
            assetBundleName = DllBundleConst.BundleName,               // "dll"
            assetNames = new[] { SrcBytes },                          // Game.dll.bytes（如有 AOT 补充 .bytes 可一起加入）
        };

        // 3. 打包（DeterministicAssetBundle 让同内容产出稳定 hash）
        var manifest = BuildPipeline.BuildAssetBundles(
            outDir,
            new[] { build },
            BuildAssetBundleOptions.DeterministicAssetBundle,
            target);

        if (manifest == null)
        {
            Debug.LogError("[BuildDllBundle] BuildAssetBundles 返回 null，打包失败");
            return;
        }

        string bundlePath = Path.Combine(outDir, DllBundleConst.BundleName);
        if (!File.Exists(bundlePath))
        {
            Debug.LogError($"[BuildDllBundle] 未生成 bundle 文件：{bundlePath}");
            return;
        }

        // 4. 打印 bundle 内实际 asset names（用于对齐 DllBundleConst.DllAssetPath）
        var loaded = AssetBundle.LoadFromFile(bundlePath);
        if (loaded != null)
        {
            foreach (var n in loaded.GetAllAssetNames())
            {
                Debug.Log($"[BuildDllBundle] bundle 内 asset: {n}");
            }
            loaded.Unload(true);
        }

        // 5. 生成 version.json（bundle + md5 + size）
        byte[] bundleBytes = File.ReadAllBytes(bundlePath);
        string md5 = ComputeMd5(bundleBytes);
        long size = bundleBytes.LongLength;
        string versionJson = $"{{\"bundle\":\"{DllBundleConst.BundleName}\",\"md5\":\"{md5}\",\"size\":{size}}}";
        string versionPath = Path.Combine(outDir, DllBundleConst.VersionFile);
        File.WriteAllText(versionPath, versionJson);

        Debug.Log($"[BuildDllBundle] 完成\n  bundle: {bundlePath}\n  md5: {md5}  size: {size}\n  version: {versionPath}");
        EditorUtility.RevealInFinder(outDir);
    }

    private static string ComputeMd5(byte[] data)
    {
        using var md5 = MD5.Create();
        byte[] hash = md5.ComputeHash(data);
        var sb = new System.Text.StringBuilder(hash.Length * 2);
        foreach (var b in hash) sb.Append(b.ToString("x2"));
        return sb.ToString();
    }
}
