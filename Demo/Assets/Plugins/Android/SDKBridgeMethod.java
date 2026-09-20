package com.demo.sdk;

import com.unity3d.player.UnityPlayer;

/**
 * Android 原生 SDK 桥接占位（空骨架）。
 *
 * C# 侧通过 JNI 调用：SDKServerAndroid.Call("Init"/"Login"/"Logout", args)
 *   → AndroidJavaClass("com.demo.sdk.SDKBridgeMethod").CallStatic(method, args)
 *
 * 原生 → C# 回调统一走：
 *   UnityPlayer.UnitySendMessage("SDKClientExecer", "OnNativeCallback", json)
 *   json 格式：{"Method":"Login","Code":10010,"Data":"..."}
 *
 * TODO: 接入真实渠道 SDK（OneSDK 等）后，在各方法里调用其 API，
 *       并在其回调里 sendCallback(...) 回传结果。
 *
 * 真机由 Gradle 编进 aar；Editor 不编译本文件。
 */
public class SDKBridgeMethod {

    private static final String GO_NAME = "SDKClientExecer";
    private static final String CALLBACK_METHOD = "OnNativeCallback";
    private static final int CODE_SUCCESS = 10010;
    private static final int CODE_FAIL = 10012;

    public static void Init(String args) {
        // TODO: 初始化真实 SDK
        sendCallback("Init", CODE_SUCCESS, "{}");
    }

    public static void Login(String args) {
        // TODO: 拉起渠道登录，登录回调里 sendCallback("Login", ...)
        sendCallback("Login", CODE_SUCCESS, "{\"uid\":\"mock\"}");
    }

    public static void Logout(String args) {
        // TODO: 登出
        sendCallback("Logout", CODE_SUCCESS, "{}");
    }

    /** 统一回调 C#：拼 JSON 后 UnitySendMessage。 */
    private static void sendCallback(String method, int code, String data) {
        String json = "{\"Method\":\"" + method + "\",\"Code\":" + code
                + ",\"Data\":" + toJsonString(data) + "}";
        UnityPlayer.UnitySendMessage(GO_NAME, CALLBACK_METHOD, json);
    }

    private static String toJsonString(String raw) {
        // data 本身是 JSON 串，转义成字符串字段
        return "\"" + raw.replace("\\", "\\\\").replace("\"", "\\\"") + "\"";
    }
}
