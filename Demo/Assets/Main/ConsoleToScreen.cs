using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 把 Console 日志画到屏幕上（真机调试用）。
/// 自绘 OnGUI，按屏幕尺寸自适应：字号随屏幕高度缩放，换行按屏幕像素宽度自动折行，
/// 不依赖任何拖入的 UI，也不再按固定字符数换行（避免窄屏被截断）。
/// </summary>
public class ConsoleToScreen : MonoBehaviour
{
    const int maxLines = 50;

    // 字号占屏幕高度的比例（0.022 ≈ 1080p 下约 24px），并有像素下限
    [Range(0.010f, 0.05f)]
    public float fontHeightRatio = 0.022f;
    public int minFontSize = 14;

    // 文字显示区域相对屏幕的边距比例（左/上/右/下都留一点边）
    public float marginRatio = 0.01f;

    // 文字颜色 & 底衬
    public Color textColor = Color.white;
    public Color backgroundColor = new Color(0f, 0f, 0f, 0.5f);

    private readonly List<string> _lines = new List<string>();
    private string _logStr = "";
    private GUIStyle _style;
    private Texture2D _bgTex;

    void OnEnable() { Application.logMessageReceived += Log; }
    void OnDisable() { Application.logMessageReceived -= Log; }

    void OnDestroy()
    {
        if (_bgTex != null) Destroy(_bgTex);
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        // 只按原始换行拆分；像素级折行交给 GUIStyle.wordWrap，不再按固定字符数切
        foreach (var line in logString.Split('\n'))
        {
            _lines.Add(line);
        }
        if (_lines.Count > maxLines)
        {
            _lines.RemoveRange(0, _lines.Count - maxLines);
        }
        _logStr = string.Join("\n", _lines);
    }

    void OnGUI()
    {
        if (string.IsNullOrEmpty(_logStr)) return;

        // 按屏幕高度算字号（真机不同分辨率自适应）
        int fontSize = Mathf.Max(minFontSize, Mathf.RoundToInt(Screen.height * fontHeightRatio));

        if (_style == null)
        {
            _style = new GUIStyle
            {
                alignment = TextAnchor.UpperLeft,
                wordWrap = true,          // 按 Rect 宽度自动折行（像素级，非字符数）
                richText = false,
            };
        }
        _style.fontSize = fontSize;
        _style.normal.textColor = textColor;

        // 显示区域：按屏幕尺寸留边，宽度决定折行位置
        float margin = Mathf.Min(Screen.width, Screen.height) * marginRatio;
        var rect = new Rect(margin, margin,
                            Screen.width - margin * 2f,
                            Screen.height - margin * 2f);

        // 半透明底衬，保证在任何背景色下都可读
        if (backgroundColor.a > 0f)
        {
            EnsureBgTex();
            float bgH = Mathf.Min(_style.CalcHeight(new GUIContent(_logStr), rect.width) + margin, rect.height);
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, bgH), _bgTex);
        }

        GUI.Label(rect, _logStr, _style);
    }

    void EnsureBgTex()
    {
        if (_bgTex != null) return;
        _bgTex = new Texture2D(1, 1);
        _bgTex.SetPixel(0, 0, backgroundColor);
        _bgTex.Apply();
    }
}
