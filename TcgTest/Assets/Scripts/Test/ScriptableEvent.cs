using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "ScriptableEvent", menuName = "ScriptableObjects/ScriptableEvent", order = 1)]
public class ScriptableEvent : ScriptableObject
{
    public UnityAction<int> action;
    public void Invoke()
    {
        action?.Invoke(2);
    }
}
