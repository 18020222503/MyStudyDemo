// using System.IO;
// using System.Reflection;
// using UnityEditor;
// using UnityEditor.Build;
// using UnityEditor.Build.Reporting;
// using UnityEngine;
// using IFix.Editor;
//
// public class IFixBuildHook : IPostBuildPlayerScriptDLLs
// {
//     public int callbackOrder { get { return 0; } }
//
//     static readonly string[] TargetAssemblies = new[]
//     {   
//         "Main.dll",
//         "Plugins.dll"
//         // "Assembly-CSharp.dll",
//         // "Assembly-CSharp-firstpass.dll"
//     };
//
//     public void OnPostBuildPlayerScriptDLLs(BuildReport report)
//     {
//         string stagingDir = FindStagingDir(report);
//         if (string.IsNullOrEmpty(stagingDir) || !Directory.Exists(stagingDir))
//         {
//             Debug.LogError("[IFixBuildHook] Cannot locate staging DLL directory, skip inject");
//             return;
//         }
//         Debug.Log("[IFixBuildHook] Staging dir: " + stagingDir);
//
//         string playerScriptAssemblies = "./Library/PlayerScriptAssemblies";
//         bool createdTempDir = false;
//         if (!Directory.Exists(playerScriptAssemblies))
//         {
//             Directory.CreateDirectory(playerScriptAssemblies);
//             createdTempDir = true;
//         }
//
//         foreach (var name in TargetAssemblies)
//         {
//             var src = Path.Combine(stagingDir, name);
//             if (!File.Exists(src)) continue;
//
//             var dst = Path.Combine(playerScriptAssemblies, name);
//             File.Copy(src, dst, true);
//
//             var pdb = Path.ChangeExtension(src, ".pdb");
//             if (File.Exists(pdb))
//                 File.Copy(pdb, Path.Combine(playerScriptAssemblies, Path.GetFileName(pdb)), true);
//         }
//
//         Debug.Log("[IFixBuildHook] Calling InjectAllAssemblys on staged DLLs");
//         IFixEditor.InjectAllAssemblys();
//
//         foreach (var name in TargetAssemblies)
//         {
//             var injected = Path.Combine(playerScriptAssemblies, name);
//             var dst = Path.Combine(stagingDir, name);
//             if (File.Exists(injected) && File.Exists(dst))
//             {
//                 File.Copy(injected, dst, true);
//                 Debug.Log("[IFixBuildHook] Copied injected DLL back: " + dst + " (size=" + new FileInfo(dst).Length + ")");
//             }
//         }
//
//         if (createdTempDir)
//         {
//             try { Directory.Delete(playerScriptAssemblies, true); } catch { }
//         }
//
//         Debug.Log("[IFixBuildHook] Inject done");
//     }
//
//     static string FindStagingDir(BuildReport report)
//     {
//         foreach (var f in report.GetFiles())
//         {
//             if (f.path.EndsWith("Assembly-CSharp.dll") && f.path.Replace('\\', '/').Contains("/Managed/"))
//             {
//                 return Path.GetDirectoryName(f.path);
//             }
//         }
//         foreach (var f in report.GetFiles())
//         {
//             if (f.path.EndsWith("Assembly-CSharp.dll"))
//             {
//                 return Path.GetDirectoryName(f.path);
//             }
//         }
//         return null;
//     }
// }
