namespace SDK
{
    /// <summary>
    /// 底层服务接口（C# → 原生 的通信原语，非业务语义）。
    /// 各平台实现：SDKServerAndroid / IOS / WX / Harmony / Editor。
    /// 上层业务（Game/SDK）通过 SDKController.SDKServer.Call(...) 复用本通道。
    /// </summary>
    public interface ISDKServer
    {
        /// <summary>初始化底层通道（建立与原生的连接、准备回调等）。</summary>
        void Start();

        /// <summary>无返回值的原生调用。method=原生方法名，args=JSON 参数。</summary>
        void Call(string method, string args = null);

        /// <summary>有返回值的同步原生调用（如取设备信息）。</summary>
        T Call<T>(string method, string args = null);
    }
}
