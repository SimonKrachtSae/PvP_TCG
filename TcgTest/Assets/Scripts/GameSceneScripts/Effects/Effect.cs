using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Effect : MonoBehaviour
{
    protected MyPlayer player;
    public MyPlayer Player { get => player; set => player = value; }
    void Start()
    {
        Card card = GetComponent<Card>();
        if (!card.isActiveAndEnabled) { this.enabled = false; return; }
        player = transform.parent.gameObject.GetComponent<MyPlayer>();
    }
    public virtual void Execute() { }
}
