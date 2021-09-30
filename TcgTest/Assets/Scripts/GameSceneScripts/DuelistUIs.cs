using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DuelistUIs
{
    private TMP_Text cardsInDeckCount;
    private TMP_Text cardsInGraveyardCount;

    private TMP_Text summonPower;

    public DuelistUIs(TMP_Text cardsInDeckCount, TMP_Text cardsInGraveyardCount, TMP_Text summonPower)
    {
        this.cardsInDeckCount = cardsInDeckCount;
        this.cardsInGraveyardCount = cardsInGraveyardCount;
        this.summonPower = summonPower;
    }

    public TMP_Text CardsInDeckCount { get => cardsInDeckCount; set => cardsInDeckCount = value; }

    public TMP_Text CardsInGraveyardCount { get => cardsInGraveyardCount; set => cardsInGraveyardCount = value; }
    public TMP_Text SummonPower { get => summonPower; set => summonPower = value; }
}

