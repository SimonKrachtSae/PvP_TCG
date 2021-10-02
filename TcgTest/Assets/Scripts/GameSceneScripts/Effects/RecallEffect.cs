using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecallEffect : Effect
{
    [SerializeField] private DuelistType target;
    [SerializeField] private NetworkTarget selector;
    [SerializeField] private MonsterCardLocation targetLocation;
    public override void Execute()
    {
        if (target == DuelistType.Enemy)
        {
            if (Game_Manager.Instance.Player != Player) Game_Manager.Instance.Player.Call_AddRecallEvents(targetLocation, selector);
            else Game_Manager.Instance.Enemy.Call_AddRecallEvents(targetLocation, selector);
        }
        else
        {
            if (Game_Manager.Instance.Player != Player) Game_Manager.Instance.Enemy.Call_AddRecallEvents(targetLocation, selector);
            else Game_Manager.Instance.Player.Call_AddRecallEvents(targetLocation, selector);
        }
    }
}

