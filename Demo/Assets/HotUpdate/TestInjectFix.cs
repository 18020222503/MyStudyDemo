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
        // Button.onClick.AddListener(OnClickButton);

    }

    void Update()
    {
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect((Screen.width - 200) / 2, 100, 200, 100), "Call  FuncA"))
        {
            Debug.Log("Button, Call FuncA, result=" + FuncA());
            Text.text = FuncA();
        }
    }
    
    [Patch]
    public string FuncA()
    {
        return "old1111";
    }
}