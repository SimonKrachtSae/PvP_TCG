using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutCardOnDeckEffect : Effect
{
    [SerializeField] private DuelistType target;
    [SerializeField] private MonsterCardLocation targetLocation;
    public override void Execute()
    {
        if (target == DuelistType.Enemy)
        {
            if (Game_Manager.Instance.Player != Player) Game_Manager.Instance.Player.SendToDeckSelected(targetLocation);
            else Game_Manager.Instance.Enemy.SendToDeckSelected(targetLocation);
        }
        else
        {
            if (Game_Manager.Instance.Player != Player) Game_Manager.Instance.Enemy.SendToDeckSelected(targetLocation);
            else Game_Manager.Instance.Player.SendToDeckSelected(targetLocation);
        }
    }
}

