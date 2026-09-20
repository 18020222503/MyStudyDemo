using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 底层 SDK 必须在热更加载之前初始化（照搬 LumiGo「SDK 早于热更」的顺序）。
        // Editor 走空实现，真机按平台工厂 new 出对应实现。
        SDK.SDKController.Init();

        HybridCLRController.Instance.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
