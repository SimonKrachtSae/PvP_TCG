using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCardStats : CardStats
{
    [SerializeField]
    private int attack;
    public int Attack { get => attack; set => attack = value; }
    private int defaultAttack;
    public int DefaultAttack { get => defaultAttack; set => defaultAttack = value; }

    [SerializeField]
    private int defense;
    public int Defense { get => defense; set => defense = value; }
    private int defaultDefense;
    public int DefaultDefense { get => defaultDefense; set => defaultDefense = value; }

    [SerializeField]
    private MonsterCardEffect effect;
    public MonsterCardEffect Effect { get => effect; set => effect = value; }

    private void Start()
    {
        DefaultAttack = attack;
        DefaultDefense = defense;
    }
    private void OnValidate()
    {
        if (attack > 10) attack = 10;
        else if (attack < 0) attack = 0;
        if (defense > 10) defense = 10;
        else if (defense < 0) defense = 0;
    }
    public void SetValuesToDefault()
    {
        attack = DefaultAttack;
        defense = DefaultDefense;
    }
}
