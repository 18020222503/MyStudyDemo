using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;

public class BuildPackage 
{

    [MenuItem("BuildPackage/Apk", false, 1)]
    public static void BuildAPK()
    {  
        var targetPath = "../BuildPackage/Apk";
        var productName = Application.productName;
        var options = new BuildPlayerOptions();
        options.scenes = new string[] { "Assets/Game.unity" };
        options.target = BuildTarget.Android;
        options.targetGroup = BuildTargetGroup.Android;
        options.options = BuildOptions.None;
        // options.options = buildParam.Development ? BuildOptions.Development : BuildOptions.None;
        options.locationPathName = Path.Combine(targetPath, productName);
        BuildPipeline.BuildPlayer(options);
    }
}
