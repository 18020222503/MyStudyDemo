namespace SDK
{
    /// <summary>
    /// 原生 → C# 的统一回调协议 DTO。
    /// 所有平台(Android/iOS/WebGL/Harmony)的原生回调都统一包成这个 JSON 结构，
    /// 经 UnitySendMessage 送回 SDKClientExecer.OnNativeCallback 后按 Method 分发。
    /// </summary>
    [System.Serializable]
    public class NativeCallbackData
    {
        /// <summary>业务方法名，用于分发到对应回调（如 "Init" / "Login" / "Logout"）。</summary>
        public string Method;

        /// <summary>结果码：CODE_SUCCESS 成功 / CODE_FAIL 失败。</summary>
        public int Code;

        /// <summary>业务数据载荷（JSON 字符串，由各业务自行解析）。</summary>
        public string Data;

        // 结果码约定（照搬 LumiGo OneSDK 约定）
        public const int CODE_SUCCESS = 10010;
        public const int CODE_FAIL = 10012;
    }
}
