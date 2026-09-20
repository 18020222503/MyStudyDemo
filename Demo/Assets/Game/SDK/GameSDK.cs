using System;
using UnityEngine;

namespace GameSDKNS
{
    /// <summary>
    /// 业务唯一静态门面。业务层只调 GameSDK.Xxx，不碰平台细节。
    /// 内部转发到 SDKScriptController.Instance.Service。
    /// </summary>
    public static class GameSDK
    {
        public static void Init(Action<bool> onComplete = null)
        {
            Debug.Log("[GameSDK] Init");
            SDKScriptController.Instance.Service.Init(onComplete);
        }

        public static void Login(Action<bool, string> onResult = null)
        {
            Debug.Log("[GameSDK] Login");
            SDKScriptController.Instance.Service.Login(onResult);
        }

        public static void Logout(Action onComplete = null)
        {
            Debug.Log("[GameSDK] Logout");
            SDKScriptController.Instance.Service.Logout(onComplete);
        }
    }
}
