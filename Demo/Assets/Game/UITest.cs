using UnityEngine;
using UnityEngine.UI;

public class UITest : MonoBehaviour
{
    // 热更类的 MonoBehaviour 不能静态挂在场景上（打包时热更 DLL 尚未加载，会变 Missing Script）。
    // 改为热更 DLL 加载成功后由代码 AddComponent 动态挂载，按钮也在运行时自己查找。
    private Button testButton;

    private void Start()
    {
        // 场景里按钮物体名为 "Button (Legacy)"
        var go = GameObject.Find("Button (Legacy)");
        if (go != null)
        {
            testButton = go.GetComponent<Button>();
        }

        if (testButton != null)
        {
            testButton.onClick.AddListener(OnTestClick);
            Debug.Log("[UITest] 已绑定按钮点击事件");
        }
        else
        {
            Debug.LogError("[UITest] 未找到按钮 'Button (Legacy)'");
        }
    }

    public void OnTestClick()
    {
        Debug.Log("OnTestClick********************");
    }
}
