using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeckData
{
    public List<CardName> CardNames;
    public DeckData(List<CardName> cardNames)
    {
        CardNames = cardNames;
    }
}
