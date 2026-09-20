using UnityEngine;

namespace SDK
{
    /// <summary>
    /// 底层 SDK 单例 + 平台工厂。
    /// 按 Application.platform + 条件编译宏 new 出对应平台的 ISDKServer 实现，
    /// 并持有统一的 ISDKClient（回调分发）。上层业务通过 SDKController.SDKServer.Call() 复用通道。
    ///
    /// 照搬 LumiGo Plugins/SDK/SDKController.cs 的模式。
    /// 由 Root.cs 在热更加载之前调用 Init()。
    /// </summary>
    public static class SDKController
    {
        public static ISDKServer SDKServer { get; private set; }
        public static ISDKClient SDKClient { get; private set; }

        private static bool _inited;

        /// <summary>初始化底层 SDK（平台工厂 + 回调收口 GameObject）。热更前调用。</summary>
        public static void Init()
        {
            if (_inited)
            {
                Debug.LogWarning("[SDKController] 已初始化，跳过");
                return;
            }
            _inited = true;

            // 统一回调客户端
            SDKClient = new SDKClient();

            // 平台工厂：按平台 new 出对应 Server 实现
#if UNITY_EDITOR
            SDKServer = new SDKServerEditor();
#elif UNITY_ANDROID
            SDKServer = new SDKServerAndroid();
#elif UNITY_IOS
            SDKServer = new SDKServerIOS();
#elif UNITY_WEBGL
            SDKServer = new SDKServerWX();
#elif UNITY_OPENHARMONY
            SDKServer = new SDKServerHarmony();
#else
            SDKServer = new SDKServerEditor();
#endif
            Debug.Log($"[SDKController] Init 平台实现 = {SDKServer.GetType().Name}");

            // 创建常驻回调收口 GameObject（承接原生 UnitySendMessage）
            SDKClientExecer.Ensure();

            // 启动底层通道
            SDKServer.Start();
        }
    }
}
