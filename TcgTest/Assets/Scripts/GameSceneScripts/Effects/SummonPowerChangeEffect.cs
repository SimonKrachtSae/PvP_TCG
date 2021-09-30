using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SummonPowerChangeEffect : Effect
{
    [SerializeField] private DuelistType targetDuelist;
    [SerializeField] private int amount;

    public int Amount { get => amount; set => amount = value; }
    public override void Execute()
    {
        if (targetDuelist == DuelistType.Enemy)
        {
            if (Game_Manager.Instance.Player != Player) Game_Manager.Instance.Player.ManaBoost += amount;
            else Game_Manager.Instance.Enemy.ManaBoost += amount;
        }
        else
        {
            if (Game_Manager.Instance.Player != Player) Game_Manager.Instance.Enemy.ManaBoost += amount;
            else Game_Manager.Instance.Player.ManaBoost += amount;
        }
    }
}
