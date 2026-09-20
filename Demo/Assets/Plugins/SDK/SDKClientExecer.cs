using UnityEngine;

namespace SDK
{
    /// <summary>
    /// 原生 → C# 回调的统一收口（挂在常驻 GameObject 上）。
    /// 原生侧统一 UnitySendMessage("SDKClientExecer", "OnNativeCallback", json) 送回，
    /// 这里反序列化成 NativeCallbackData 后交给 SDKController.SDKClient 分发。
    ///
    /// GameObject 名字必须与原生侧 UnitySendMessage 的第一个参数一致：见各原生桥接文件。
    /// </summary>
    public class SDKClientExecer : MonoBehaviour
    {
        /// <summary>与原生 UnitySendMessage 约定的 GameObject 名。</summary>
        public const string GO_NAME = "SDKClientExecer";

        private static SDKClientExecer _instance;

        /// <summary>确保场景中存在常驻收口对象。</summary>
        public static void Ensure()
        {
            if (_instance != null) return;
            var go = new GameObject(GO_NAME);
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<SDKClientExecer>();
        }

        /// <summary>原生统一回调入口（被 UnitySendMessage 调用，方法名不可改）。</summary>
        public void OnNativeCallback(string json)
        {
            Debug.Log($"[SDKClientExecer] OnNativeCallback: {json}");
            if (string.IsNullOrEmpty(json)) return;

            NativeCallbackData data;
            try
            {
                data = JsonUtility.FromJson<NativeCallbackData>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SDKClientExecer] 回调 JSON 解析失败: {e.Message}\n{json}");
                return;
            }

            SDKController.SDKClient?.Dispatch(data);
        }
    }
}
