# SDK 五端两层空骨架 说明

参照商业项目 LumiGo 的 SDK 架构，在本 demo 搭的**空骨架**（接口留空、方法体待填）。
核心模式：**面向接口 + 平台工厂 + 统一回调收口**。

## 分层

```
业务层（热更 Game）         Hello.Run() 只调 GameSDK.Init/Login
        │
上层 Game/SDK（热更）       GameSDK(门面) → SDKScriptController(平台工厂) → ISDKService
        │  业务语义翻译成底层 Call
底层 Plugins/SDK（AOT）     SDKController(平台工厂) → ISDKServer.Call(method,args)
        │  JNI / DllImport / jslib / etslib
各平台原生（占位）          Plugins/{Android,iOS,WebGL,OpenHarmony}/
```

## 文件清单

### 底层 `Assets/Plugins/SDK/`（AOT 程序集 Plugins，命名空间 `SDK`）
- `NativeCallbackData.cs` — 统一回调协议 `{Method,Code,Data}`，码 10010 成功 / 10012 失败
- `ISDKServer.cs` / `ISDKClient.cs` — 底层服务/客户端接口
- `SDKClient.cs` — 回调分发（Method→回调 字典）
- `SDKController.cs` — **底层单例 + 平台工厂**（`SDKController.Init()`）
- `SDKClientExecer.cs` — `UnitySendMessage` 统一收口（GameObject 名 `SDKClientExecer`）
- `SDKAssistant.cs` — iOS DllImport extern 声明集中处
- `Server/SDKServer{Editor,Android,IOS,WX,Harmony}.cs` — 各端底层实现

### 上层 `Assets/Game/SDK/`（热更程序集 Game，命名空间 `GameSDKNS`）
- `ISDKService.cs` — 业务接口 Init/Login/Logout
- `SDKScriptController.cs` — **上层单例 + 平台工厂**
- `GameSDK.cs` — **业务唯一静态门面**
- `Service/SDKServices.cs` — 公共基类 + 各端业务实现

### 原生占位
- `Plugins/Android/SDKBridgeMethod.java`（`com.demo.sdk`）
- `Plugins/iOS/GameSDKBridge.mm`
- `Plugins/WebGL/SDKBridge.jslib`
- `Plugins/OpenHarmony/OneSDKBridge.etslib`（纯占位）

## 启动顺序（照搬 LumiGo「SDK 早于热更」）
`Root.Start()`：`SDKController.Init()`（底层，平台工厂 + 收口）→ `HybridCLRController.Start()`（加载热更）
→ 热更入口 `Hello.Run()` 调 `GameSDK.Init → Login`。

## 数据流（以 Login 为例）
```
GameSDK.Login(cb)
 → SDKScriptController.Service.Login(cb)
 → SDKClient.Register("Login", 收结果)  +  SDKServer.Call("Login")
 → [原生] 拉起登录，完成后 UnitySendMessage("SDKClientExecer","OnNativeCallback", json)
 → SDKClientExecer.OnNativeCallback → SDKClient.Dispatch → 触发 cb
```
Editor 下 `SDKServerEditor.Call` 直接模拟成功回调，整条链路可在编辑器跑通。

## 怎么往里加新能力（如 Pay）
1. 上层 `ISDKService` 加 `Pay(...)`，`SDKServiceBase` 实现（Register "Pay" + Call "Pay"）。
2. 门面 `GameSDK` 加 `Pay(...)` 转发。
3. 各原生占位加对应方法 + sendCallback。
无需改平台工厂结构。

## 注意
- 原生文件仅对应平台参与编译，Editor 不受影响（靠 Unity 插件平台导入设置 / `#if` 宏）。
- 首次导入后，需在 Unity Inspector 里确认各原生插件的平台设置（Android→Android、iOS→iOS、jslib→WebGL）。
- `.etslib` 为纯占位，真机鸿蒙构建时替换为真实库。
