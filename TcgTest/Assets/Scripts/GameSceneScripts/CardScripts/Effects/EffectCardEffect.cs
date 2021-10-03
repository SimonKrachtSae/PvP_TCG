using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EffectCardEffect : MonoBehaviour
{
    private UnityAction onPlay;
    public UnityAction OnPlay { get => onPlay; set => onPlay = value; }
    [SerializeField] private List<Effect> effects;
    private void Awake()
    {
        for (int i = 0; i < effects.Count; i++)
        {
            OnPlay += effects[i].Execute;
        }
    }
}
