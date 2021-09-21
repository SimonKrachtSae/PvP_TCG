using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardStats : ScriptableObject
{
    public int Index { get; set; }

    [SerializeField]
    private string cardName;
    public string CardName { get => cardName; set => cardName = value; }

    public MonsterCardLocation MonsterCardLocation { get; set; }

    [SerializeField]
    private CardEffect effect;
    public CardEffect Effect { get => effect; set => effect = value; }

    [SerializeField]
    private int playCost;
    public int PlayCost { get => playCost; set => playCost = value; }
    private void OnValidate()
    {
        
    }
}

