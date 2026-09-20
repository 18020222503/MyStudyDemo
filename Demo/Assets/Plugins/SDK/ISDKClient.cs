using System;

namespace SDK
{
    /// <summary>
    /// 底层客户端接口：负责注册/注销原生回调，并在收到原生回调时按 Method 分发。
    /// </summary>
    public interface ISDKClient
    {
        /// <summary>注册某个 Method 的回调处理。</summary>
        void Register(string method, Action<int, string> callback);

        /// <summary>注销某个 Method 的回调。</summary>
        void Unregister(string method);

        /// <summary>收到原生回调时分发（由 SDKClientExecer 调用）。</summary>
        void Dispatch(NativeCallbackData data);
    }
}
