using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private List<MonsterCard> monsterCards;
    public List<MonsterCard> MonsterCards
    { 
        get => monsterCards;
        set => monsterCards = value;
    }
}
