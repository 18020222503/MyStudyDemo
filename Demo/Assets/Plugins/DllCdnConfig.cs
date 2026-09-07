/// <summary>
/// CDN 下载地址配置（当前写死，方便本地起 HTTP 服务器测试）。
/// 后续需要服务端下发时，把 CdnRoot 改成运行时赋值即可，调用方接口不变。
///
/// 目录约定：bundle 按平台分目录存放，实际下载地址为 CdnRoot/&lt;平台&gt;/xxx。
/// 打包输出目录为 E:\study\Demo\Bundle\&lt;平台&gt;\（见 BuildDllBundle），
/// 平台名由 DllBundleConst.GetPlatformName / GetRuntimePlatformName 统一给出，两端一致。
/// </summary>
public static class DllCdnConfig
{
    // 本地测试：在 bundle 输出根目录起 HTTP 服务器，让根对应 CdnRoot，例如：
    //   cd /e/study/Demo/Bundle && python -m http.server 8080
    // 这样客户端会去 http://.../Android/version.json 等按平台子目录取文件。
    // 当前使用本机局域网 IP（真机需与本机处于同一局域网且可互通）。
    public const string CdnRoot = "http://10.27.239.98:8080";
}
