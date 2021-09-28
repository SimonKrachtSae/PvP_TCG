using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SummonPowerChangeEffect : Effect
{
    [SerializeField] private DuelistType duelistType;
    [SerializeField] private int amount;

    public int Amount { get => amount; set => amount = value; }
    public override void Execute()
    {
        if (duelistType == DuelistType.Enemy)
        {
            if (Game_Manager.Instance.Player != Player) Game_Manager.Instance.Player.SummonPowerBoost += amount;
            else Game_Manager.Instance.Enemy.SummonPowerBoost += amount;
        }
        else
        {
            if (Game_Manager.Instance.Player != Player) Game_Manager.Instance.Enemy.SummonPowerBoost += amount;
            else Game_Manager.Instance.Player.SummonPowerBoost += amount;
        }
    }
}
