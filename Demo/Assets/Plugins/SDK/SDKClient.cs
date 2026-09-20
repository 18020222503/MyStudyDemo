using System;
using System.Collections.Generic;
using UnityEngine;

namespace SDK
{
    /// <summary>
    /// 底层客户端默认实现：维护 Method → 回调 的字典，收到原生回调后查表分发。
    /// </summary>
    public class SDKClient : ISDKClient
    {
        private readonly Dictionary<string, Action<int, string>> _callbacks
            = new Dictionary<string, Action<int, string>>();

        public void Register(string method, Action<int, string> callback)
        {
            if (string.IsNullOrEmpty(method) || callback == null) return;
            _callbacks[method] = callback;
        }

        public void Unregister(string method)
        {
            if (string.IsNullOrEmpty(method)) return;
            _callbacks.Remove(method);
        }

        public void Dispatch(NativeCallbackData data)
        {
            if (data == null || string.IsNullOrEmpty(data.Method))
            {
                Debug.LogWarning("[SDKClient] 收到空回调或缺 Method，忽略");
                return;
            }

            if (_callbacks.TryGetValue(data.Method, out var cb))
            {
                cb?.Invoke(data.Code, data.Data);
            }
            else
            {
                Debug.LogWarning($"[SDKClient] 未注册 Method={data.Method} 的回调");
            }
        }
    }
}
