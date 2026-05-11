using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITest :MonoBehaviour
{

    public Button Button;

    void Start()
    {
        Button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        TestInjectPlugins t = new TestInjectPlugins();
        t.print();
    }
}
