using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    int i = 0;
    [SerializeField] private List<CardStats> monsterCards;
    public List<CardStats> MonsterCards
    { 
        get => monsterCards;
        set => monsterCards = value;
    }
}
