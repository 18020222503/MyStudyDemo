using System.Collections;
using System.Collections.Generic;
using IFix;
using UnityEngine;

public class TestInjectPlugins 
{
    [Patch]
    public void print()
    {
        Debug.Log("2222222");
    }
}
