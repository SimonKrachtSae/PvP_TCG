using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "EffectCardEffect", menuName = "ScriptableObjects/EffectCardEffect", order = 3)]
public class EffectCardEffect : ScriptableObject
{
    private UnityAction onPlay;
    public UnityAction OnPlay { get => onPlay; set => onPlay = value; }
    [SerializeField] private List<Effect> effects;
    private void OnValidate()
    {
        OnPlay = null;
        for (int i = 0; i < effects.Count; i++) OnPlay += effects[i].Execute;
    }
}
