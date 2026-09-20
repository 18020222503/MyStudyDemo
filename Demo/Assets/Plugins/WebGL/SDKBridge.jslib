// WebGL / 小游戏 原生 SDK 桥接占位（空骨架）。
//
// C# 侧通过 DllImport(__Internal) 调用（声明见 SDKServerWX.cs）：
//   JS_SDK_Init / JS_SDK_Login / JS_SDK_Logout
//
// 原生 → C# 回调统一走 SendMessage：
//   SendMessage("SDKClientExecer", "OnNativeCallback", json)
//   json 格式：{"Method":"Login","Code":10010,"Data":"..."}
//
// TODO: 接入微信/抖音小游戏 OneSDK JS 后，在各函数里调用其 API，
//       并在其回调里 sendCallback(...) 回传结果。
//
// 仅 WebGL 平台参与编译。

mergeInto(LibraryManager.library, {

  JS_SDK_Init: function (argsPtr) {
    // var args = UTF8ToString(argsPtr);
    // TODO: 初始化真实小游戏 SDK
    _sdkSendCallback('Init', 10010, '{}');
  },

  JS_SDK_Login: function (argsPtr) {
    // TODO: 拉起小游戏登录，回调里 _sdkSendCallback('Login', ...)
    _sdkSendCallback('Login', 10010, '{"uid":"mock"}');
  },

  JS_SDK_Logout: function (argsPtr) {
    // TODO: 登出
    _sdkSendCallback('Logout', 10010, '{}');
  },

  // 统一回调 C#
  _sdkSendCallback: function (method, code, data) {
    var json = JSON.stringify({ Method: method, Code: code, Data: data });
    SendMessage('SDKClientExecer', 'OnNativeCallback', json);
  },
});
