namespace SDK
{
    /// <summary>
    /// iOS 原生方法的 DllImport 声明集中处（对应 Plugins/iOS/GameSDKBridge.mm 里的 extern "C" 函数）。
    /// 仅真机 iOS 参与编译；Editor 下不声明，避免链接不到 __Internal。
    /// </summary>
    public static class SDKAssistant
    {
#if UNITY_IOS && !UNITY_EDITOR
        [System.Runtime.InteropServices.DllImport("__Internal")]
        public static extern void SDK_Init(string args);

        [System.Runtime.InteropServices.DllImport("__Internal")]
        public static extern void SDK_Login(string args);

        [System.Runtime.InteropServices.DllImport("__Internal")]
        public static extern void SDK_Logout(string args);
#endif
    }
}
