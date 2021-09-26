using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldMonstersAttackChange : Effect
{
    [SerializeField] private DuelistType duelistType;
    [SerializeField] private int amount;

    public int Amount { get => amount; set => amount = value; }
    public override void Execute()
    {
        if (duelistType == DuelistType.Enemy)
        {
            if (Game_Manager.Instance.Player != Player) Apply(Game_Manager.Instance.Player);
            else Apply(Game_Manager.Instance.Enemy);
        }
        else
        {
            if (Game_Manager.Instance.Player != Player) Apply(Game_Manager.Instance.Enemy);
            else Apply(Game_Manager.Instance.Player);
        }
    }
    private void Apply(MyPlayer target)
    {
        for(int i = 0; i < target.Field.Count; i++)
        {
            ((MonsterCardStats)target.Field[i].CardStats).Attack += amount;
            target.Field[i].DrawValues();
        }
    }
}
