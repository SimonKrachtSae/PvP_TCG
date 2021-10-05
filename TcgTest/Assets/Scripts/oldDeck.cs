using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oldDeck : MonoBehaviour
{
	public string Name = "Deck 1";
    [SerializeField] private List<MonsterCardStats> monsterCards;
    public List<MonsterCardStats> MonsterCards
    { 
        get => monsterCards;
        set => monsterCards = value;
    }
}
