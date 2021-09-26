using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCard : Effect
{
    [SerializeField] private DuelistType duelistType;
    [SerializeField] private int amount;

    public int Amount { get => amount; set => amount = value; }
    public override void Execute()
    {
        if (duelistType == DuelistType.Enemy)
        {
            if (Game_Manager.Instance.Player != Player) for (int i = 0; i < amount; i++) Game_Manager.Instance.Player.DrawCard(0);
            else for (int i = 0; i < amount; i++) Game_Manager.Instance.Enemy.DrawCard(0);
        }
        else
        {
            if (Game_Manager.Instance.Player != Player) for (int i = 0; i < amount; i++) Game_Manager.Instance.Enemy.DrawCard(0);
            else for (int i = 0; i < amount; i++) Game_Manager.Instance.Player.DrawCard(0);
        }
    }
}
