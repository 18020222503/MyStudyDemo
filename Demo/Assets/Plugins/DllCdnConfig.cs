/// <summary>
/// CDN 下载地址配置（当前写死，方便本地起 HTTP 服务器测试）。
/// 后续需要服务端下发时，把 CdnRoot 改成运行时赋值即可，调用方接口不变。
/// </summary>
public static class DllCdnConfig
{
    // 本地测试：在 bundle 输出目录（BuildBundles/<平台>）起 HTTP 服务器，例如：
    //   cd BuildBundles/Android && python -m http.server 8080
    // 真机测试注意：不能用 127.0.0.1（那是手机自己），要用运行 HTTP 服务器那台电脑的局域网 IP，
    // 例如 http://192.168.1.100:8080
    public const string CdnRoot = "http://127.0.0.1:8080";
}
