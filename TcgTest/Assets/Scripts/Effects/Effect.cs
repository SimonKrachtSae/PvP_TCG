using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Effect : MonoBehaviour
{
    protected MyPlayer player;
    public MyPlayer Player { get => player; set => player = value; }
    void Awake()
    {
        player = transform.parent.gameObject.GetComponent<MyPlayer>();
    }
    public virtual void Execute() { }
}
