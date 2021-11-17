using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DuelistUIs
{
    private TMP_Text cardsInDeckCount;
    private TMP_Text cardsInGraveyardCount;

    private Transform manaPos;

    public DuelistUIs(TMP_Text cardsInDeckCount, TMP_Text cardsInGraveyardCount, Transform manaPos)
    {
        this.cardsInDeckCount = cardsInDeckCount;
        this.cardsInGraveyardCount = cardsInGraveyardCount;
        this.manaPos = manaPos;
    }

    public TMP_Text CardsInDeckCount { get => cardsInDeckCount; set => cardsInDeckCount = value; }

    public TMP_Text CardsInGraveyardCount { get => cardsInGraveyardCount; set => cardsInGraveyardCount = value; }
    public Transform ManaPos { get => manaPos; set => manaPos = value; }
}

