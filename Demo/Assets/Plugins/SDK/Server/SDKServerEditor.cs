using UnityEngine;

namespace SDK
{
    /// <summary>
    /// 编辑器空实现：不接原生，直接打印日志并模拟成功回调。
    /// 方便在 Editor 里跑通整条链路。
    /// </summary>
    public class SDKServerEditor : ISDKServer
    {
        public void Start()
        {
            Debug.Log("[SDKServerEditor] Start (编辑器空实现)");
        }

        public void Call(string method, string args = null)
        {
            Debug.Log($"[SDKServerEditor] Call {method} args={args}");

            // 模拟原生异步回调：直接回一个成功（走统一分发通道）
            var data = new NativeCallbackData
            {
                Method = method,
                Code = NativeCallbackData.CODE_SUCCESS,
                Data = "{\"mock\":true}"
            };
            SDKController.SDKClient?.Dispatch(data);
        }

        public T Call<T>(string method, string args = null)
        {
            Debug.Log($"[SDKServerEditor] Call<{typeof(T).Name}> {method} args={args}");
            return default;
        }
    }
}
