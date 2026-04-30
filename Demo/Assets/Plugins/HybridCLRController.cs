using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Reflection;

public class HybridCLRController
{
    public static readonly HybridCLRController Instance = new HybridCLRController();

    Assembly _plugins;
    Assembly _Main;
    Assembly _hotUpdateAss;

    public void Start()
    {
        if (_hotUpdateAss != null) return;

// #if UNITY_EDITOR
        // _plugins = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "Plugins");
        // _Main = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "Main");
        // _hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotUpdate");
// #else
        _plugins = Assembly.Load(File.ReadAllBytes($"{Application.streamingAssetsPath}/Plugins.dll.bytes"));
        _Main = Assembly.Load(File.ReadAllBytes($"{Application.streamingAssetsPath}/Main.dll.bytes"));
        _hotUpdateAss = Assembly.Load(File.ReadAllBytes($"{Application.streamingAssetsPath}/HotUpdate.dll.bytes"));
// #endif

       Debug.Log("111111:" + _hotUpdateAss == null);
       Debug.Log("22222" + _plugins == null);
       Debug.Log("33333:" + _Main == null);

        // Type helloType = _hotUpdateAss.GetType("Hello");
        // if (helloType == null)
        // {
        //     Debug.LogError("Hello type not found");
        //     return;
        // }
        //
        // MethodInfo printMethod = helloType.GetMethod("Print");
        // printMethod?.Invoke(null, null);
    }

    public void Update()
    {
    }
}
