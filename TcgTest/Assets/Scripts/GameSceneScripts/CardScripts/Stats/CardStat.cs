using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardStat : MonoBehaviour
{
    [SerializeField] private int maxCount;
    public int MaxCount { get => maxCount; set => maxCount = value; }
    public int Index { get; set; }

    [SerializeField]
    protected string cardName;
    public string CardName { get => cardName; set => cardName = value; }

    [SerializeField]
    protected int playCost;
    public int PlayCost { get => playCost; set => playCost = value; }

    [SerializeField]
    protected string effectText;
    public string EffectText { get => effectText; set => effectText = value; }
}

