using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardStats : MonoBehaviour
{
    public int Index { get; set; }

    [SerializeField]
    private string cardName;
    public string CardName { get => cardName; set => cardName = value; }

    [SerializeField]
    private int playCost;
    public int PlayCost { get => playCost; set => playCost = value; }
}

