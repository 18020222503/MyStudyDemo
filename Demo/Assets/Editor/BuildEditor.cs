using System.Collections.Generic;
using System.IO;
using IFix.Editor;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildEditor
{
    const string BUILD_OUTPUT = "Build/PC";
    const string EXE_NAME = "Inject.exe";

    [MenuItem("Build/PC", false, 1)]
    public static void BuildPC()
    {
        var scenes = GetEnabledScenes();
        if (scenes.Length == 0)
        {
            Debug.LogError("No scenes in Build Settings!");
            return;
        }

        string outputPath = Path.Combine(BUILD_OUTPUT, EXE_NAME);
        string dir = Path.GetDirectoryName(outputPath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputPath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        Debug.Log("---> Start Build PC (InjectFix auto-injects via PostProcessScene during this build)");
        var report = BuildPipeline.BuildPlayer(options);

        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"---> Build Succeeded: {report.summary.outputPath} (size: {report.summary.totalSize} bytes)");
            System.Diagnostics.Process.Start("Explorer.exe", Path.GetFullPath(dir).Replace('/', '\\'));
        }
        else
        {
            Debug.LogError($"---> Build Failed: {report.summary.result}");
        }
    }

    [MenuItem("Build/Inject", false, 1)]
    public static void Inject()
    {
        IFixEditor.InjectAssembly("HotUpdate");
    }

    static string[] GetEnabledScenes()
    {
        var scenes = new List<string>();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                scenes.Add(scene.path);
            }
        }
        return scenes.ToArray();
    }
}
