// iOS 原生 SDK 桥接占位（空骨架）。
//
// C# 侧通过 DllImport(__Internal) 调用（声明见 SDKAssistant.cs）：
//   SDK_Init / SDK_Login / SDK_Logout
//
// 原生 → C# 回调统一走 UnitySendMessage：
//   UnitySendMessage("SDKClientExecer", "OnNativeCallback", json)
//   json 格式：{"Method":"Login","Code":10010,"Data":"..."}
//
// TODO: 接入真实渠道 SDK（OneSDK 等）后，在各函数里调用其 API，
//       并在其回调里 SendUnityCallback(...) 回传结果。
//
// 仅 iOS 平台参与编译（由 Unity 的插件平台导入设置控制）。

#import <Foundation/Foundation.h>

extern "C" {
    void UnitySendMessage(const char* obj, const char* method, const char* msg);
}

static const char* kGoName = "SDKClientExecer";
static const char* kCallback = "OnNativeCallback";
static const int kCodeSuccess = 10010;
static const int kCodeFail = 10012;

// 统一回调 C#
static void SendUnityCallback(NSString* method, int code, NSString* data) {
    NSString* json = [NSString stringWithFormat:
        @"{\"Method\":\"%@\",\"Code\":%d,\"Data\":\"%@\"}",
        method, code, (data ?: @"{}")];
    UnitySendMessage(kGoName, kCallback, [json UTF8String]);
}

extern "C" {

    void SDK_Init(const char* args) {
        // TODO: 初始化真实 SDK
        SendUnityCallback(@"Init", kCodeSuccess, @"{}");
    }

    void SDK_Login(const char* args) {
        // TODO: 拉起渠道登录，登录回调里 SendUnityCallback(@"Login", ...)
        SendUnityCallback(@"Login", kCodeSuccess, @"{\\\"uid\\\":\\\"mock\\\"}");
    }

    void SDK_Logout(const char* args) {
        // TODO: 登出
        SendUnityCallback(@"Logout", kCodeSuccess, @"{}");
    }
}
