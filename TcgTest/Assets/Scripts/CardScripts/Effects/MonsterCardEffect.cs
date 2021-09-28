using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class MonsterCardEffect : MonoBehaviour
{
    private UnityAction onSummon;
    public UnityAction OnSummon { get => onSummon; set => onSummon = value; }
    [SerializeField] private List<Effect> onSummonEffects;
    public UnityAction OnDestroy { get; set; }
    [SerializeField] private List<Effect> onDestroyEffects;
    private void Start()
    {
        for(int i = 0; i < onSummonEffects.Count; i++) OnSummon += onSummonEffects[i].Execute;
        for(int i = 0; i < onDestroyEffects.Count; i++) OnDestroy += onDestroyEffects[i].Execute;
    }
}
