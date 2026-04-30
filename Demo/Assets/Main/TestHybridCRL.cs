using System.Collections;
using System.Collections.Generic;
using System.IO;
using IFix;
using IFix.Core;
using UnityEngine;
using UnityEngine.UI;

public class TestHybridCRL : MonoBehaviour
{
    public Button Button;
    public Text Text;

    void Start()
    {
        Button.onClick.AddListener(OnClick);
    }

    void Update()
    {
        
    }

    private void OnClick()
    {
        Text.text = Test.print();
    }

}