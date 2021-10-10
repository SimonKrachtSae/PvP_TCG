using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackChangeEffect : Effect
{
    [SerializeField] private DuelistType duelistType;
    [SerializeField] private int amount;
    [SerializeField] private AttackTarget target;
    public override void Execute()
    {
        if (duelistType == DuelistType.Enemy)
        {
            if (Game_Manager.Instance.Player != Player)
            { 
                foreach (MonsterCard card in Game_Manager.Instance.Player.Field)
                {
                    if (target == AttackTarget.All)
                    {
                        ((MonsterCardStats)card.CardStats).Attack += amount;
                        ((MonsterCard_Layout)card.Layout).AttackTextUI.text = ((MonsterCardStats)card.CardStats).Attack.ToString();
                    }
                    else
                    {
                        if (card.gameObject.GetComponent<AttackChangeEffect>() == this)
                        {
                            ((MonsterCardStats)card.CardStats).Attack += amount;
                            ((MonsterCard_Layout)card.Layout).AttackTextUI.text = ((MonsterCardStats)card.CardStats).Attack.ToString();
                            return;
                        }
                    }
                }
            }
            else
            {
                foreach (MonsterCard card in Game_Manager.Instance.Enemy.Field)
                {
                    if (target == AttackTarget.All)
                    {
                        ((MonsterCardStats)card.CardStats).Attack += amount;
                        ((MonsterCard_Layout)card.Layout).AttackTextUI.text = ((MonsterCardStats)card.CardStats).Attack.ToString();
                    }
                    else
                    {
                        if (card.gameObject.GetComponent<AttackChangeEffect>() == this)
                        {
                            ((MonsterCardStats)card.CardStats).Attack += amount;
                            ((MonsterCard_Layout)card.Layout).AttackTextUI.text = ((MonsterCardStats)card.CardStats).Attack.ToString();
                            return;
                        }
                    }
                }
            }
        }
        else
        {
            if (Game_Manager.Instance.Player != Player)
            {
                foreach (MonsterCard card in Game_Manager.Instance.Enemy.Field)
                {
                    if (target == AttackTarget.All)
                    {
                        ((MonsterCardStats)card.CardStats).Attack += amount;
                        ((MonsterCard_Layout)card.Layout).AttackTextUI.text = ((MonsterCardStats)card.CardStats).Attack.ToString();
                    }
                    else
                    {
                        if (card.gameObject.GetComponent<AttackChangeEffect>() == this)
                        {
                            ((MonsterCardStats)card.CardStats).Attack += amount;
                            ((MonsterCard_Layout)card.Layout).AttackTextUI.text = ((MonsterCardStats)card.CardStats).Attack.ToString();
                            return;
                        }
                    }
                }
            }
            else
            {
                foreach (MonsterCard card in Game_Manager.Instance.Player.Field)
                {
                    if (target == AttackTarget.All)
                    {
                        ((MonsterCardStats)card.CardStats).Attack += amount;
                        ((MonsterCard_Layout)card.Layout).AttackTextUI.text = ((MonsterCardStats)card.CardStats).Attack.ToString();
                    }
                    else
                    {
                        if (card.gameObject.GetComponent<AttackChangeEffect>() == this)
                        {
                            ((MonsterCardStats)card.CardStats).Attack += amount;
                            ((MonsterCard_Layout)card.Layout).AttackTextUI.text = ((MonsterCardStats)card.CardStats).Attack.ToString();
                            return;
                        }
                    }
                }
            }
        }
    }
}
public enum AttackTarget
{
    This,
    All
}
