using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITest : MonoBehaviour
{
    
    public Button  testButton;


    private void Start()
    {
        testButton.onClick.AddListener(OnTestClick);
    }


    public void OnTestClick()
    {
        Debug.Log("OnTestClick********************");
    }
}
