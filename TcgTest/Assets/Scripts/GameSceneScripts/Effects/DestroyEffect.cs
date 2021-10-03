using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffect : Effect
{
    [SerializeField] private DuelistType target;
    [SerializeField] private NetworkTarget selector;
    [SerializeField] private int amount;
    public override void Execute()
    {
        if (target == DuelistType.Enemy)
        {
            if (Game_Manager.Instance.Player != Player) Game_Manager.Instance.Player.Call_AddDestroyEffects(amount, selector);
            else Game_Manager.Instance.Enemy.Call_AddDestroyEffects(amount, selector);
        }
        else
        {
            if (Game_Manager.Instance.Player != Player) Game_Manager.Instance.Enemy.Call_AddDestroyEffects(amount, selector);
            else Game_Manager.Instance.Player.Call_AddDestroyEffects(amount, selector);
        }
    }
}
