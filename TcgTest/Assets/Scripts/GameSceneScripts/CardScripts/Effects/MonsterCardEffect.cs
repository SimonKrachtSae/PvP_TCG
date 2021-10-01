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
    public UnityAction OnAttack { get; set; }
    [SerializeField] private List<Effect> onAttackEffects;
    public UnityAction OnBlock { get; set; }
    [SerializeField] private List<Effect> onBlockEffects;
    public UnityAction OnDirectAttackSucceeds { get; set; }
    [SerializeField] private List<Effect> onDirectAttackSucceedsEffects;
    private void Start()
    {
        for(int i = 0; i < onSummonEffects.Count; i++) OnSummon += onSummonEffects[i].Execute;
        for(int i = 0; i < onDestroyEffects.Count; i++) OnDestroy += onDestroyEffects[i].Execute;
        for (int i = 0; i < onAttackEffects.Count; i++) OnAttack += onAttackEffects[i].Execute;
        for (int i = 0; i < onBlockEffects.Count; i++) OnBlock += onBlockEffects[i].Execute;
        for (int i = 0; i < onDirectAttackSucceedsEffects.Count; i++) OnDirectAttackSucceeds += onDirectAttackSucceedsEffects[i].Execute;
    }
}
