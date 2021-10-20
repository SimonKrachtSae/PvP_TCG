using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGameManager: MonoBehaviour
{
    public bool b;
    private Arrow a;
    public KeyValuePair<object,object> pair;
    private void OnValidate()
    {
        pair = new KeyValuePair<object, object>("S", 2);
    }
}
