using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardStats : ScriptableObject
{
    [SerializeField]
    private string cardName;
    public string CardName { get => cardName; set => cardName = value; }

    [SerializeField]
    private Effect effect;
    public Effect Effect { get => effect; set => effect = value; }

    [SerializeField]
    private int playCost;
    public int PlayCost { get => playCost; set => playCost = value; }
}

