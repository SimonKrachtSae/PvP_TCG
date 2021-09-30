using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCardStats : CardStats
{
    [SerializeField]
    private EffectCardEffect effect;
    public EffectCardEffect Effect { get => effect; set => effect = value; }
}
