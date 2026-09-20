using System;
using SDK;
using UnityEngine;

namespace GameSDKNS
{
    /// <summary>
    /// 上层业务实现的公共基类：把业务语义(Init/Login/Logout)统一翻译成
    /// 底层 SDKController.SDKServer.Call + SDKClient.Register 收结果。
    /// 各平台子类只需提供平台名（差异化逻辑可 override）。
    /// </summary>
    public abstract class SDKServiceBase : ISDKService
    {
        protected abstract string PlatformTag { get; }

        public virtual void Init(Action<bool> onComplete)
        {
            Debug.Log($"[{PlatformTag}] Init");
            SDKController.SDKClient.Register("Init", (code, data) =>
            {
                SDKController.SDKClient.Unregister("Init");
                onComplete?.Invoke(code == NativeCallbackData.CODE_SUCCESS);
            });
            SDKController.SDKServer.Call("Init");
        }

        public virtual void Login(Action<bool, string> onResult)
        {
            Debug.Log($"[{PlatformTag}] Login");
            SDKController.SDKClient.Register("Login", (code, data) =>
            {
                SDKController.SDKClient.Unregister("Login");
                onResult?.Invoke(code == NativeCallbackData.CODE_SUCCESS, data);
            });
            SDKController.SDKServer.Call("Login");
        }

        public virtual void Logout(Action onComplete)
        {
            Debug.Log($"[{PlatformTag}] Logout");
            SDKController.SDKClient.Register("Logout", (code, data) =>
            {
                SDKController.SDKClient.Unregister("Logout");
                onComplete?.Invoke();
            });
            SDKController.SDKServer.Call("Logout");
        }
    }

    public class SDKServiceAndroid : SDKServiceBase { protected override string PlatformTag => "SDKServiceAndroid"; }
    public class SDKServiceIOS     : SDKServiceBase { protected override string PlatformTag => "SDKServiceIOS"; }
    public class SDKServiceWX      : SDKServiceBase { protected override string PlatformTag => "SDKServiceWX"; }
    public class SDKServiceHarmony : SDKServiceBase { protected override string PlatformTag => "SDKServiceHarmony"; }
    public class SDKServiceEditor  : SDKServiceBase { protected override string PlatformTag => "SDKServiceEditor"; }
}
