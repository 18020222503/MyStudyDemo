using UnityEngine;

namespace SDK
{
    /// <summary>
    /// iOS 底层实现：通过 DllImport(__Internal) 调用原生 Objective-C 方法。
    /// extern 声明集中在 SDKAssistant，实现在 Plugins/iOS/GameSDKBridge.mm。
    /// 仅真机 iOS 参与实际调用。
    /// </summary>
    public class SDKServerIOS : ISDKServer
    {
        public void Start()
        {
            Debug.Log("[SDKServerIOS] Start");
        }

        public void Call(string method, string args = null)
        {
#if UNITY_IOS && !UNITY_EDITOR
            switch (method)
            {
                case "Init":   SDKAssistant.SDK_Init(args);   break;
                case "Login":  SDKAssistant.SDK_Login(args);  break;
                case "Logout": SDKAssistant.SDK_Logout(args); break;
                default:
                    Debug.LogWarning($"[SDKServerIOS] 未映射的 method={method}");
                    break;
            }
#else
            Debug.Log($"[SDKServerIOS] (非真机) Call {method} args={args}");
#endif
        }

        public T Call<T>(string method, string args = null)
        {
            // TODO: iOS 同步返回值调用（如取设备信息），需在 .mm 里返回字符串再解析
            Debug.Log($"[SDKServerIOS] Call<{typeof(T).Name}> {method}（未实现）");
            return default;
        }
    }
}
