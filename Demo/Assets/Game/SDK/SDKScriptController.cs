using UnityEngine;

namespace GameSDKNS
{
    /// <summary>
    /// 上层业务单例 + 平台工厂（与底层 SDKController 同构）。
    /// 按平台 new 出对应 ISDKService 实现，业务门面 GameSDK 通过它转发。
    /// 属热更程序集（Game），运行时才创建。
    /// </summary>
    public class SDKScriptController
    {
        private static SDKScriptController _instance;
        public static SDKScriptController Instance => _instance ??= new SDKScriptController();

        public ISDKService Service { get; private set; }

        private SDKScriptController()
        {
            // 平台工厂：按平台 new 出对应业务实现
#if UNITY_EDITOR
            Service = new SDKServiceEditor();
#elif UNITY_ANDROID
            Service = new SDKServiceAndroid();
#elif UNITY_IOS
            Service = new SDKServiceIOS();
#elif UNITY_WEBGL
            Service = new SDKServiceWX();
#elif UNITY_OPENHARMONY
            Service = new SDKServiceHarmony();
#else
            Service = new SDKServiceEditor();
#endif
            Debug.Log($"[SDKScriptController] 业务实现 = {Service.GetType().Name}");
        }
    }
}
