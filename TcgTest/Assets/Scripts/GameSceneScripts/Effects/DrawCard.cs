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
            if (Game_Manager.Instance.Player != Player) Game_Manager.Instance.Player.Call_DrawCards(amount);
            else Game_Manager.Instance.Enemy.Call_DrawCards(amount);
        }
        else
        {
            if (Game_Manager.Instance.Player != Player) Game_Manager.Instance.Enemy.Call_DrawCards(amount);
            else Game_Manager.Instance.Player.Call_DrawCards(amount);
        }
    }
}
