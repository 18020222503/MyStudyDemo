using System.Collections;
using System.Collections.Generic;
using System.IO;
using IFix;
using IFix.Core;
using UnityEngine;
using UnityEngine.UI;

public class TestInjectFix : MonoBehaviour
{
    public Button Button;
    public Text Text;

    void Start()
    {
    }

    void Update()
    {
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect((Screen.width - 300) / 2, 100, 300, 80), "Benchmark"))
        {
            RunBenchmark();
        }
    }

    void RunBenchmark()
    {
        int iterations = 10;
    
        // Patch 版本（解释执行）
        var sw = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
            TestInjectFixPlugins.Calculate();
        sw.Stop();
        long patchMs = sw.ElapsedMilliseconds;
        Debug.Log($"[Patch] Calculate: {patchMs}ms ({iterations}次)");

        // 原生版本（AOT 编译执行）
        sw.Restart();
        for (int i = 0; i < iterations; i++)
            TestInjectFixPlugins.CalculateNative();
        sw.Stop();
            
        
        long nativeMs = sw.ElapsedMilliseconds;
        Debug.Log($"[Native] CalculateNative: {nativeMs}ms ({iterations}次)");

        float ratio = nativeMs > 0 ? (float)patchMs / nativeMs : (float)patchMs;
        Debug.Log($">>> 性能对比: Patch是Native的 {ratio:F4} 倍耗时");
        
        

    }
   
    [Patch]
    public string FuncA()
    {
        return "11";
    }
}
