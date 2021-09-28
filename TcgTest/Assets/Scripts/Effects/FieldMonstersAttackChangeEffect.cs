using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldMonstersAttackChangeEffect : Effect
{
    [SerializeField] private DuelistType duelistType;
    [SerializeField] private int amount;

    public int Amount { get => amount; set => amount = value; }
    public override void Execute()
    {
        if (duelistType == DuelistType.Enemy)
        {
            if (Game_Manager.Instance.Player != Player) Game_Manager.Instance.Player.AttackBoost += amount;
            else Game_Manager.Instance.Enemy.AttackBoost += amount;
        }
        else
        {
            if (Game_Manager.Instance.Player != Player) Game_Manager.Instance.Enemy.AttackBoost += amount;
            else Game_Manager.Instance.Player.AttackBoost += amount;
        }
    }
}
