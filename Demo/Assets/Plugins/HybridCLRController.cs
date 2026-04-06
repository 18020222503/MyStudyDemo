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
    public  void Start()
    {       
        if (_hotUpdateAss == null)
        {
#if UNITY_EDITOR
            _hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotUpdate");
#else
            _hotUpdateAss = Assembly.Load(File.ReadAllBytes($"{Application.streamingAssetsPath}/HotUpdate.dll.bytes"));
#endif      
        }
        
        Type helloType = _hotUpdateAss.GetType("Hello");
        MethodInfo runMethod = helloType.GetMethod("Run");
        runMethod.Invoke(null, null);

    }
    
    public void Update()
    {
        
    }
    
    
    
    
    
    Assembly _hotUpdateAss;
}
