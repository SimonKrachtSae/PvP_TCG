using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterCard", menuName = "ScriptableObjects/MonsterCard", order = 1)]
public class MonsterCard : Card
{
    [SerializeField]
    private int attack;
    public int Attack { get => attack; set => attack = value; }

    [SerializeField]
    private int defense;
    public int Defense { get => attack; set => attack = value; }
    private void OnValidate()
    {
        if (attack > 10) attack = 10;
        else if (attack < 0) attack = 0;
        if (defense > 10) defense = 10;
        else if (defense < 0) defense = 0;
    }
}
