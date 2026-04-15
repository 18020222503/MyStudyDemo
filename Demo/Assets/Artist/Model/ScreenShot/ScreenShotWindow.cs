using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class ScreenShotWindow : EditorWindow
{
    private enum ResolutionPreset
    {
        CurrentGameView,
        Preset2K,
        Preset4K,
        Preset8K,
        Preset16K,
    }

    private enum CaptureColorFormat
    {
        ARGB32,
        ARGBFloat,
        ARGBHalf,
    }

    private static readonly string[] ResolutionOptions =
    {
        "当前GameView",
        "2K (2048x2048)",
        "4K (4096x4096)",
        "8K (8192x8192)",
        "16K (16384x16384)",
    };

    private static readonly string[] CaptureFormatOptions =
    {
        "ARGB32",
        "ARGBFloat",
        "ARGBHalf",
    };

    private Camera m_Camera;
    private string filePath;
    private bool m_IsEnableAlpha = false;
    private CameraClearFlags m_CameraClearFlags;
    private ResolutionPreset m_ResolutionPreset = ResolutionPreset.Preset8K;
    private CaptureColorFormat m_CaptureColorFormat = CaptureColorFormat.ARGBFloat;

    [MenuItem("Tools/屏幕截图")]
    private static void Init()
    {
        ScreenShotWindow window = GetWindowWithRect<ScreenShotWindow>(new Rect(0, 0, 320, 210));
        window.titleContent = new GUIContent("屏幕截图");
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        m_Camera = EditorGUILayout.ObjectField("选择摄像机", m_Camera, typeof(Camera), true) as Camera;
        m_ResolutionPreset = (ResolutionPreset)EditorGUILayout.Popup("分辨率", (int)m_ResolutionPreset, ResolutionOptions);
        m_CaptureColorFormat = (CaptureColorFormat)EditorGUILayout.Popup("颜色格式", (int)m_CaptureColorFormat, CaptureFormatOptions);

        if (GUILayout.Button("保存位置"))
        {
            filePath = EditorUtility.OpenFolderPanel("", "", "");
        }

        m_IsEnableAlpha = EditorGUILayout.Toggle("是否开启透明通道", m_IsEnableAlpha);
        EditorGUILayout.Space();
        if (GUILayout.Button("截图"))
        {
            TakeShot();
        }
        EditorGUILayout.Space();
        if (GUILayout.Button("打开导出文件夹"))
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogError("<color=red>" + "没有选择截图保存位置" + "</color>");
                return;
            }
            Application.OpenURL("file://" + filePath);
        }
    }

    private void TakeShot()
    {
        if (m_Camera == null)
        {
            Debug.LogError("<color=red>" + "没有选择摄像机" + "</color>");
            return;
        }

        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("<color=red>" + "没有选择截图保存位置" + "</color>");
            return;
        }

        Vector2Int resolution = GetResolution(m_ResolutionPreset);
        RenderTextureFormat captureFormat = GetRenderTextureFormat(m_CaptureColorFormat);
        if (!SystemInfo.SupportsRenderTextureFormat(captureFormat))
        {
            Debug.LogError("<color=red>" + $"当前设备不支持截图颜色格式: {captureFormat}" + "</color>");
            return;
        }

        m_CameraClearFlags = m_Camera.clearFlags;
        RenderTexture previousActive = RenderTexture.active;
        RenderTexture previousTargetTexture = m_Camera.targetTexture;
        RenderTexture captureTexture = null;
        RenderTexture readbackTexture = null;
        Texture2D screenShot = null;

        if (m_IsEnableAlpha)
        {
            m_Camera.clearFlags = CameraClearFlags.Depth;
        }

        try
        {
            captureTexture = new RenderTexture(resolution.x, resolution.y, 24, captureFormat);
            captureTexture.Create();
            readbackTexture = new RenderTexture(resolution.x, resolution.y, 0, RenderTextureFormat.ARGB32);
            readbackTexture.Create();

            m_Camera.targetTexture = captureTexture;
            screenShot = new Texture2D(resolution.x, resolution.y, TextureFormat.ARGB32, false);

            m_Camera.Render();
            Graphics.Blit(captureTexture, readbackTexture);
            RenderTexture.active = readbackTexture;
            screenShot.ReadPixels(new Rect(0, 0, resolution.x, resolution.y), 0, 0);

            byte[] bytes = screenShot.EncodeToPNG();
            string fileName = Path.Combine(filePath, $"{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png");
            File.WriteAllBytes(fileName, bytes);
            Debug.Log("截图成功");
        }
        finally
        {
            m_Camera.targetTexture = previousTargetTexture;
            RenderTexture.active = previousActive;
            m_Camera.clearFlags = m_CameraClearFlags;

            if (captureTexture != null)
            {
                captureTexture.Release();
                DestroyImmediate(captureTexture);
            }

            if (readbackTexture != null)
            {
                readbackTexture.Release();
                DestroyImmediate(readbackTexture);
            }

            if (screenShot != null)
            {
                DestroyImmediate(screenShot);
            }
        }
    }

    private static Vector2Int GetResolution(ResolutionPreset preset)
    {
        switch (preset)
        {
            case ResolutionPreset.Preset2K:
                return new Vector2Int(2048, 2048);
            case ResolutionPreset.Preset4K:
                return new Vector2Int(4096, 4096);
            case ResolutionPreset.Preset8K:
                return new Vector2Int(8192, 8192);
            case ResolutionPreset.Preset16K:
                return new Vector2Int(16384, 16384);
            default:
                Vector2 gameViewSize = Handles.GetMainGameViewSize();
                return new Vector2Int((int)gameViewSize.x, (int)gameViewSize.y);
        }
    }

    private static RenderTextureFormat GetRenderTextureFormat(CaptureColorFormat captureColorFormat)
    {
        switch (captureColorFormat)
        {
            case CaptureColorFormat.ARGBFloat:
                return RenderTextureFormat.ARGBFloat;
            case CaptureColorFormat.ARGBHalf:
                return RenderTextureFormat.ARGBHalf;
            default:
                return RenderTextureFormat.ARGB32;
        }
    }
}
#endif
