using UnityEngine;

namespace SDK
{
    /// <summary>
    /// Android 底层实现：通过 JNI(AndroidJavaClass.CallStatic) 调用原生静态方法。
    /// 对应原生类 com.demo.sdk.SDKBridgeMethod（见 Plugins/Android/SDKBridgeMethod.java）。
    /// 仅真机 Android 参与编译。
    /// </summary>
    public class SDKServerAndroid : ISDKServer
    {
        // 原生 Java 类全名（照搬 LumiGo：Java 类名可按渠道 CN/海外切换，这里先固定）
        private const string ANDROID_CLASS = "com.demo.sdk.SDKBridgeMethod";

        public void Start()
        {
            Debug.Log("[SDKServerAndroid] Start");
            // TODO: 如需向原生传初始化配置，可在此 Call("Start", ...)
        }

        public void Call(string method, string args = null)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using var jc = new AndroidJavaClass(ANDROID_CLASS);
                jc.CallStatic(method, args);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SDKServerAndroid] Call {method} 失败: {e}");
            }
#else
            Debug.Log($"[SDKServerAndroid] (非真机) Call {method} args={args}");
#endif
        }

        public T Call<T>(string method, string args = null)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using var jc = new AndroidJavaClass(ANDROID_CLASS);
                return jc.CallStatic<T>(method, args);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SDKServerAndroid] Call<{typeof(T).Name}> {method} 失败: {e}");
                return default;
            }
#else
            return default;
#endif
        }
    }
}
