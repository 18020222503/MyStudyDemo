using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using HybridCLR;

/// <summary>
/// 从 CDN 下载热更 DLL 的 AssetBundle 并加载出 Game 程序集。
/// 流程：下 version.json → 比对本地缓存 md5 → 变了才下 bundle 落地 →
///       LoadFromFileAsync → 取 TextAsset(Game.dll) → 补 AOT 元数据 → Assembly.Load。
/// </summary>
public class DllBundleLoader : MonoBehaviour
{
    [Serializable]
    private class VersionInfo
    {
        public string bundle;
        public string md5;
        public long size;
    }

    private string CacheDir => Path.Combine(Application.persistentDataPath, DllBundleConst.CacheDir);
    private string LocalBundlePath => Path.Combine(CacheDir, DllBundleConst.BundleName);
    private string LocalVersionPath => Path.Combine(CacheDir, DllBundleConst.VersionFile);

    /// <summary>
    /// 启动下载+加载。完成后回调加载到的 Game 程序集（失败为 null）。
    /// </summary>
    public void Load(Action<Assembly> onComplete)
    {
        StartCoroutine(LoadRoutine(onComplete));
    }

    private IEnumerator LoadRoutine(Action<Assembly> onComplete)
    {
        Directory.CreateDirectory(CacheDir);

        // CDN 上按平台分目录存放（与打包输出目录 Bundle/<平台>/ 对齐）
        string cdnBase = $"{DllCdnConfig.CdnRoot}/{DllBundleConst.GetRuntimePlatformName()}";

        // 1. 下 version.json
        string remoteVerUrl = $"{cdnBase}/{DllBundleConst.VersionFile}";
        VersionInfo remoteVer = null;
        using (var req = UnityWebRequest.Get(remoteVerUrl))
        {
            yield return req.SendWebRequest();
            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[DllBundleLoader] 下载 version.json 失败: {req.error} url={remoteVerUrl}");
                onComplete?.Invoke(null);
                yield break;
            }
            remoteVer = JsonUtility.FromJson<VersionInfo>(req.downloadHandler.text);
        }
        if (remoteVer == null || string.IsNullOrEmpty(remoteVer.md5))
        {
            Debug.LogError("[DllBundleLoader] version.json 解析失败");
            onComplete?.Invoke(null);
            yield break;
        }

        // 2. 比对本地缓存
        bool needDownload = true;
        if (File.Exists(LocalBundlePath) && File.Exists(LocalVersionPath))
        {
            var localVer = JsonUtility.FromJson<VersionInfo>(File.ReadAllText(LocalVersionPath));
            // 本地 md5 与远端一致，且本地 bundle 实际 md5 也对得上（防缓存被改坏）
            if (localVer != null && localVer.md5 == remoteVer.md5 &&
                ComputeMd5(File.ReadAllBytes(LocalBundlePath)) == remoteVer.md5)
            {
                needDownload = false;
                Debug.Log("[DllBundleLoader] 本地缓存 md5 命中，跳过下载");
            }
        }

        // 3. 需要则下 bundle
        if (needDownload)
        {
            string bundleUrl = $"{cdnBase}/{remoteVer.bundle}";
            Debug.Log($"[DllBundleLoader] 下载 bundle: {bundleUrl}");
            using (var req = UnityWebRequest.Get(bundleUrl))
            {
                yield return req.SendWebRequest();
                if (req.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"[DllBundleLoader] 下载 bundle 失败: {req.error}");
                    onComplete?.Invoke(null);
                    yield break;
                }
                byte[] data = req.downloadHandler.data;
                // 校验 md5
                if (ComputeMd5(data) != remoteVer.md5)
                {
                    Debug.LogError("[DllBundleLoader] 下载的 bundle md5 校验不通过");
                    onComplete?.Invoke(null);
                    yield break;
                }
                File.WriteAllBytes(LocalBundlePath, data);
                File.WriteAllText(LocalVersionPath, JsonUtility.ToJson(remoteVer));
            }
        }

        // 4. 加载 bundle
        var abReq = AssetBundle.LoadFromFileAsync(LocalBundlePath);
        yield return abReq;
        AssetBundle bundle = abReq.assetBundle;
        if (bundle == null)
        {
            Debug.LogError("[DllBundleLoader] LoadFromFile 得到空 bundle");
            onComplete?.Invoke(null);
            yield break;
        }

        // 5. 取出 Game.dll 的 TextAsset 字节（bundle 内只有热更相关 .bytes）
        Assembly hotUpdateAss = null;
        try
        {
            byte[] gameBytes = LoadDllBytes(bundle, DllBundleConst.HotUpdateDll);
            if (gameBytes == null)
            {
                Debug.LogError("[DllBundleLoader] 未在 bundle 中找到 Game.dll 的 TextAsset");
            }
            else
            {
                // 6. 先补 AOT 补充元数据（当前名单为空则跳过；有 AOT dll 时它们也需打进同一 bundle）
                foreach (var aotName in AOTGenericReferences.PatchedAOTAssemblyList)
                {
                    byte[] aotBytes = LoadDllBytes(bundle, aotName);
                    if (aotBytes == null)
                    {
                        Debug.LogError($"[DllBundleLoader] AOT 补充元数据缺失: {aotName}");
                        continue;
                    }
                    var err = RuntimeApi.LoadMetadataForAOTAssembly(aotBytes, HomologousImageMode.SuperSet);
                    if (err != LoadImageErrorCode.OK)
                        Debug.LogError($"[DllBundleLoader] LoadMetadataForAOTAssembly 失败: {aotName} err={err}");
                }

                // 7. 加载热更程序集
                hotUpdateAss = Assembly.Load(gameBytes);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[DllBundleLoader] 加载程序集异常: {e}");
        }

        bundle.Unload(false); // 保留已从 bundle 读出的 TextAsset 数据（已 memcpy 到 byte[]）
        onComplete?.Invoke(hotUpdateAss);
    }

    /// <summary>
    /// 从 bundle 里按 DLL 名取对应 TextAsset 的字节。
    /// TextAsset.name 为去掉扩展名后的文件名（Game.dll.bytes -> "Game.dll"），据此匹配。
    /// </summary>
    private static byte[] LoadDllBytes(AssetBundle bundle, string dllName)
    {
        var all = bundle.LoadAllAssets<TextAsset>();
        foreach (var ta in all)
        {
            if (ta != null && ta.name == dllName)
                return ta.bytes;
        }
        return null;
    }

    private static string ComputeMd5(byte[] data)
    {
        using var md5 = MD5.Create();
        byte[] hash = md5.ComputeHash(data);
        var sb = new StringBuilder(hash.Length * 2);
        foreach (var b in hash) sb.Append(b.ToString("x2"));
        return sb.ToString();
    }
}
