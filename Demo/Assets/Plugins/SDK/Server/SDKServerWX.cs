using System.Runtime.InteropServices;
using UnityEngine;

namespace SDK
{
    /// <summary>
    /// WebGL / 小游戏底层实现：通过 .jslib 的 extern 函数与 JS 层通信。
    /// jslib 实现见 Plugins/WebGL/SDKBridge.jslib。仅 WebGL 参与实际调用。
    /// </summary>
    public class SDKServerWX : ISDKServer
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern void JS_SDK_Init(string args);
        [DllImport("__Internal")] private static extern void JS_SDK_Login(string args);
        [DllImport("__Internal")] private static extern void JS_SDK_Logout(string args);
#endif

        public void Start()
        {
            Debug.Log("[SDKServerWX] Start");
        }

        public void Call(string method, string args = null)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            switch (method)
            {
                case "Init":   JS_SDK_Init(args);   break;
                case "Login":  JS_SDK_Login(args);  break;
                case "Logout": JS_SDK_Logout(args); break;
                default:
                    Debug.LogWarning($"[SDKServerWX] 未映射的 method={method}");
                    break;
            }
#else
            Debug.Log($"[SDKServerWX] (非 WebGL) Call {method} args={args}");
#endif
        }

        public T Call<T>(string method, string args = null)
        {
            Debug.Log($"[SDKServerWX] Call<{typeof(T).Name}> {method}（未实现）");
            return default;
        }
    }
}
