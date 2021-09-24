using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectCard", menuName = "ScriptableObjects/EffectCard", order = 2)]
public class EffectCardStats : CardStats
{
    [SerializeField]
    private EffectCardEffect effect;
    public EffectCardEffect Effect { get => effect; set => effect = value; }
}
