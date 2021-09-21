using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "CardEffect", menuName = "ScriptableObjects/CardEffect", order = 1)]
public class CardEffect : ScriptableObject
{
    private UnityAction onSummon;
    public UnityAction OnSummon { get => onSummon; set { onSummon = value; Debug.Log("HOA"); } }
    [SerializeField] private List<Effect> onSummonEffects;
    public UnityAction OnDestroy { get; set; }
    [SerializeField] private List<Effect> onDestroyEffects;
    public Effect SummonEffect;
    public Effect DestroyEffect;
    private void OnValidate()
    {
        Debug.Log("DDD");
        OnSummon = null;
        OnDestroy = null;

        for(int i = 0; i < onSummonEffects.Count; i++) OnSummon += onSummonEffects[i].Execute;
        for(int i = 0; i < onDestroyEffects.Count; i++) OnDestroy += onDestroyEffects[i].Execute;
    }
}
