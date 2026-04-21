using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITest:MonoBehaviour
{
    public Button Button;
    public Text Text;
    void Start()
    {
        Button.onClick.AddListener(OnClickButton);
    }

    void Update()
    {
        
    }

    void OnClickButton()
    {
        Text.text = "OnClickButton";
        Debug.Log("OnClickButton");
    }
}
