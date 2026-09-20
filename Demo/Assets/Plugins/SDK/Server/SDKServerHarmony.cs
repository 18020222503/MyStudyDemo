using UnityEngine;

namespace SDK
{
    /// <summary>
    /// OpenHarmony(鸿蒙) 底层实现占位。
    /// 鸿蒙通过 .etslib / .har 与 ArkTS 层通信（见 Plugins/OpenHarmony/OneSDKBridge.etslib）。
    /// 当前工程无 UNITY_OPENHARMONY 宏时，此类仅作占位、不参与真实调用。
    /// </summary>
    public class SDKServerHarmony : ISDKServer
    {
        public void Start()
        {
            Debug.Log("[SDKServerHarmony] Start（鸿蒙占位）");
        }

        public void Call(string method, string args = null)
        {
            // TODO: 接鸿蒙 OneSDK .etslib，通过 host 侧 API 调用 ArkTS 方法
            Debug.Log($"[SDKServerHarmony] Call {method} args={args}（占位）");
        }

        public T Call<T>(string method, string args = null)
        {
            Debug.Log($"[SDKServerHarmony] Call<{typeof(T).Name}> {method}（占位）");
            return default;
        }
    }
}
