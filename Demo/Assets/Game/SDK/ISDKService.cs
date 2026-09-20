using System;

namespace GameSDKNS
{
    /// <summary>
    /// 上层业务接口（业务语义：初始化 / 登录 / 登出）。
    /// 各平台实现把业务语义翻译成底层 SDKController.SDKServer.Call(...)。
    /// 骨架先预留 Init/Login/Logout，后续按需扩展 Pay/Share/Track 等。
    /// </summary>
    public interface ISDKService
    {
        /// <summary>SDK 初始化，完成回调 ok=是否成功。</summary>
        void Init(Action<bool> onComplete);

        /// <summary>登录，回调 (ok, userInfoJson)。</summary>
        void Login(Action<bool, string> onResult);

        /// <summary>登出。</summary>
        void Logout(Action onComplete);
    }
}
